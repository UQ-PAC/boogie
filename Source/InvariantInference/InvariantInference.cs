using System;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.Boogie.VCExprAST;
using Microsoft.Boogie.SMTLib;
using Microsoft.BaseTypes;
using Microsoft.Boogie.GraphUtil;
using VC;

namespace Microsoft.Boogie.InvariantInference {
  public class InvariantInference {

    public static void RunInvariantInference(Program program) {
      Dictionary<Procedure, Implementation[]> procedureImplementations = ComputeProcImplMap(program);

      ProverInterface prover = ProverInterface.CreateProver(program, CommandLineOptions.Clo.ProverLogFilePath,
        CommandLineOptions.Clo.ProverLogFileAppend, CommandLineOptions.Clo.TimeLimit, true);

      InterpolationProver interpol = InterpolationProver.CreateProver(program, CommandLineOptions.Clo.InterpolationLogFilePath,
        CommandLineOptions.Clo.InterpolationLogFileAppend, CommandLineOptions.Clo.TimeLimit);

      
      if (CommandLineOptions.Clo.InterpolantSolverKind == CommandLineOptions.InterpolantSolver.Princess &&
        CommandLineOptions.Clo.InterpolationQETactic == CommandLineOptions.QETactic.princess) {
        prover = interpol;
      }
      
      // get bv op functions

      SortedDictionary<(string op, int size), Function> BitVectorOpFunctions = new SortedDictionary<(string, int), Function>();
      foreach (Function f in program.Functions) {
        if (f.Attributes != null && f.Attributes.Key == "bvbuiltin") {
          string op = (f.Attributes.Params[0] as string);
          int size = f.InParams[0].TypedIdent.Type.BvBits;
          BitVectorOpFunctions.Add((op, size), f);
        }
      }

      List<Function> newBVFunctions = new List<Function>();
      
      Dictionary<Function, VCExpr> functionDefs = new Dictionary<Function, VCExpr>();

      List<VCExpr> QFAxioms = new List<VCExpr>();
      
      // collect all axioms without quantifiers to use in interpolation
      // in future, could add all axioms with ONLY exists and skolemise them? 
      foreach (Axiom a in program.Axioms) {
        ForallCollector collector = new ForallCollector();
        collector.Visit(a.Expr);
        List<ForallExpr> foralls = collector.foralls;
        if (foralls.Count == 0) {
          QFAxioms.Add(prover.Context.BoogieExprTranslator.Translate(a.Expr));
        }

        // TODO: deal with triggers more directly using quantifiers in axioms found here, instead of current lookup heuristic
      }

      // pretty naive & inefficient, assuming one axiom to instantiate per function at most and only fitting specific forall pattern, but it's a start
      // better for future is to use triggers found with the ForallCollector loop above, but then having to figure out instantiating quantifiers of 
      // more complex patterns is required
      foreach (Function f in program.Functions) {
        if (f.Body == null && f.DefinitionBody == null) {
          if (f.DefinitionAxiom != null) {
            if (f.DefinitionAxiom.Expr is ForallExpr) {
              Expr body = ((ForallExpr)f.DefinitionAxiom.Expr).Body;
              VCExpr forall = prover.Context.BoogieExprTranslator.Translate(f.DefinitionAxiom.Expr);
              functionDefs.Add(f, forall);
            }
          } else {
            foreach (Axiom a in program.Axioms) {
              if (a.Expr is ForallExpr) {
                Expr body = ((ForallExpr)a.Expr).Body;
                if (ExprContainsFunc.Check(body, f.Name)) {
                  VCExpr forall = prover.Context.BoogieExprTranslator.Translate(a.Expr);
                  functionDefs.Add(f, forall);
                  break;
                }
              }
            }
          }
        }
      }
     


      // analyze each procedure
      foreach (var proc in program.Procedures) {
        if (procedureImplementations.ContainsKey(proc)) {
          // analyze each implementation of the procedure
          foreach (var impl in procedureImplementations[proc]) {
            // naive attempt at getting variables that may be referred to
            // want variables in terms of original implementation
            IEnumerable<Variable> scopeVars = new List<Variable>();
            scopeVars = scopeVars.Concat(program.GlobalVariables);
            scopeVars = scopeVars.Concat(impl.LocVars);
            scopeVars = scopeVars.Concat(impl.InParams);
            scopeVars = scopeVars.Concat(impl.OutParams);
            foreach (Block b in impl.Blocks) {
              // add desugared variables
              foreach (Cmd c in b.Cmds) {
                if (c is CallCmd) {
                  scopeVars = scopeVars.Concat(((StateCmd)((CallCmd)c).Desugaring).Locals);
                }
              }
            }

            // predecessors are often not up-to-date at this point, surprisingly
            impl.ComputePredecessorsForBlocks();

            // need copy as we will perform transformations on the loop/s
            Duplicator duplicator = new Duplicator();
            Implementation implCopy = duplicator.VisitImplementation(impl);

            // duplicator does not copy predecessors properly
            implCopy.ComputePredecessorsForBlocks();

            Graph<Block> g = Program.GraphFromImpl(implCopy);
            g.ComputeLoops();
            if (!g.Reducible) {
              throw new Exception("Irreducible flow graphs are unsupported.");
            }

            List<Block> loopHeads = g.Headers.ToList();
            if (loopHeads.Count == 0) {
              continue;
            }
            Block loopHead = loopHeads[0];

            List<Block> originalLoopHeads = new List<Block>();
            foreach (Block orig in impl.Blocks) {
              foreach (Block b in loopHeads) {
                if (b.Label == orig.Label) {
                  originalLoopHeads.Add(orig);
                }
              }
            }


            if (CommandLineOptions.Clo.TraceVerify) {
              Console.WriteLine("before flattening multiple loops");
              ConditionGeneration.EmitImpl(implCopy, false);
            }

            if (loopHeads.Count > 1) {
              // do we need to insert new start block first?
              /*
              impl.Blocks.Insert(0,
                new Block(new Token(-17, -4), "0", new List<Cmd>(),
                  new GotoCmd(Token.NoToken, new List<String> { impl.Blocks[0].Label }, new List<Block> { impl.Blocks[0] })));
              impl.Blocks[1].Predecessors.Add(impl.Blocks[0]);
              */

              //if (CommandLineOptions.Clo.TraceVerify) {
              //  Console.WriteLine("before flattening multiple loops");
              //  ConditionGeneration.EmitImpl(impl, false);
              //}

              LocalVariable blockVar = new LocalVariable(Token.NoToken, new TypedIdent(Token.NoToken, "_@block", Type.Int));
              implCopy.LocVars.Add(blockVar);
              IdentifierExpr blockId = new IdentifierExpr(Token.NoToken, blockVar);
              int id = 0;
              HashSet<Block> loopHeadSuccessors = new HashSet<Block>();
              // we are counting any edge to a loop head from within or after a loop as a back edge, as it needs to become a back edge as part of this transformation
              // this is different to existing back edges found in graph
              HashSet<Block> loopBodyBackEdgeNodes = new HashSet<Block>();
              Dictionary<string, int> loopLabelIds = new Dictionary<String, int>();
              foreach (Block head in g.Headers) {
                loopLabelIds.Add(head.Label, id);
                LiteralExpr idLiteral = new LiteralExpr(Token.NoToken, BigNum.FromInt(id));

                foreach (Block successor in head.Exits()) {
                  loopHeadSuccessors.Add(successor);
                  // at start of every block that a loophead points to, add assume for that loophead's block value (can be cleverer about this if needed & directly and with existing assumes?)
                  var kv = new QKeyValue(Token.NoToken, "partition", new List<object>(), null);
                  Expr e = Expr.Eq(blockId, idLiteral);
                  e.Typecheck(new TypecheckingContext(null));
                  successor.Cmds.Insert(0, new AssumeCmd(Token.NoToken, e, kv));
                }
                foreach (Block predecessor in head.Predecessors) {
                  foreach (Block head2 in g.SortHeadersByDominance()) {
                    if (g.DominatorMap.DominatedBy(predecessor, head2)) {
                      loopBodyBackEdgeNodes.Add(predecessor);
                      break;
                    }
                  }
                  // before each goto to a loophead(forward or back), assign block variable with value for destination block
                  predecessor.Cmds.Add(Cmd.SimpleAssign(Token.NoToken, blockId, idLiteral));
                }
                id++;
              }

              // create new block that merges all loopheads - goto all possible locations from a loophead
              GotoCmd UnifiedHeadGoto = new GotoCmd(Token.NoToken, loopHeadSuccessors.ToList());
              Block UnifiedLoopHead = new Block(Token.NoToken, "UnifiedLoopHead", new List<Cmd>(), UnifiedHeadGoto);
              implCopy.Blocks.Add(UnifiedLoopHead);

              // need to take into account case where loophead is not empty - deal with later
              // another thing to handle - multiple exits from loop - unify them!

              // create new block to collect all backedges and point to unified loophead
              GotoCmd UnifiedBackGoto = new GotoCmd(Token.NoToken, new List<Block> { UnifiedLoopHead });
              // assert @block >= 0 && @block < loopHeadCount

              Expr blockBounds = Expr.And(Expr.Ge(blockId, new LiteralExpr(Token.NoToken, BigNum.ZERO)), Expr.Lt(blockId, new LiteralExpr(Token.NoToken, BigNum.FromInt(id))));
              blockBounds.Typecheck(new TypecheckingContext(null));
              AssertCmd assertBlockBounds = new AssertCmd(Token.NoToken, blockBounds);
              Block UnifiedLoopBack = new Block(Token.NoToken, "UnifiedLoopBack", new List<Cmd> { assertBlockBounds }, UnifiedBackGoto);
              implCopy.Blocks.Add(UnifiedLoopBack);

              foreach (Block head in g.Headers) {
                foreach (Block predecessor in head.Predecessors) {
                  GotoCmd transfer = predecessor.TransferCmd as GotoCmd;
                  transfer.labelTargets.Remove(head);
                  transfer.labelNames.Remove(head.Label);
                  if (loopBodyBackEdgeNodes.Contains(predecessor)) {
                    // replace all loophead gotos from within the loop body with goto backedge collect block
                    transfer.AddTarget(UnifiedLoopBack);
                  } else {
                    // replace all loophead gotos from before the loop goto unified loophead block
                    transfer.AddTarget(UnifiedLoopHead);
                  }
                }
                implCopy.Blocks.Remove(head);
              }
              implCopy.ComputePredecessorsForBlocks();

              if (CommandLineOptions.Clo.TraceVerify) {
                Console.WriteLine("after flattening multiple loops");
                ConditionGeneration.EmitImpl(impl, false);
              }

              loopHead = UnifiedLoopHead;

              // add block var to original
              foreach (Block b in originalLoopHeads) {

                int bId = loopLabelIds[b.Label];
                // block is a loop head
                LiteralExpr idLiteral = new LiteralExpr(Token.NoToken, BigNum.FromInt(bId));

                // add invariant for each loophead representing its value
                Expr invar = Expr.Eq(blockId, idLiteral);
                b.Cmds.Insert(0, new AssertCmd(Token.NoToken, invar));

                foreach (Block successor in b.Exits()) {
                  // at start of every block that a loophead points to, add assume for that loophead's block value (can be cleverer about this if needed & directly and with existing assumes?)
                  var kv = new QKeyValue(Token.NoToken, "partition", new List<object>(), null);
                  Expr e = Expr.Eq(blockId, idLiteral);
                  successor.Cmds.Insert(0, new AssumeCmd(Token.NoToken, e, kv));
                }
                foreach (Block predecessor in b.Predecessors) {
                  // before each goto to a loophead(forward or back), assign block variable with value for destination block
                  predecessor.Cmds.Add(Cmd.SimpleAssign(Token.NoToken, blockId, idLiteral));
                }

              }

              impl.LocVars.Add(blockVar);
            }

            InvariantInferrer inferrer = new InvariantInferrer(program, procedureImplementations, implCopy, loopHead, prover, interpol, BitVectorOpFunctions, newBVFunctions, functionDefs, QFAxioms);
            if (CommandLineOptions.Clo.InterpolationDebugLevel == CommandLineOptions.InterpolationDebug.Stats ||
              CommandLineOptions.Clo.InterpolationDebugLevel == CommandLineOptions.InterpolationDebug.None) {
              try {
                VCExpr invariant = inferrer.InferLoopInvariant();
                Instrument(originalLoopHeads, invariant, scopeVars, BitVectorOpFunctions);
              } catch {
                // do clean up if execution is thrown and we're not debugging
                prover.Close();
                interpol.Close();
                throw;
              }
            } else {
              // have to manually clean up if debugging so that stack trace is visible
              VCExpr invariant = inferrer.InferLoopInvariant();
              Instrument(originalLoopHeads, invariant, scopeVars, BitVectorOpFunctions);
            }

          }
        }
      }
      prover.Close();
      interpol.Close();
      program.AddTopLevelDeclarations(newBVFunctions);

    }

    private static void Instrument(List<Block> loopHeads, VCExpr invariant, IEnumerable<Variable> scopeVars, SortedDictionary<(string op, int size), Function> bvOps) {
      // need to translate VCExpr back into Expr - kind of terrible
      Expr inv = VCtoExpr(invariant, scopeVars, bvOps);
      var kv = new QKeyValue(Token.NoToken, "inferred", new List<object>(), null);
      PredicateCmd cmd = new AssertCmd(Token.NoToken, inv, kv);
      foreach (Block b in loopHeads) {
        b.cmds.Insert(0, cmd);
      }
    }

    // more robust would be to use visitor?
    // also need to take into account stack overflows potentially? 
    private static Expr VCtoExpr(VCExpr vc, IEnumerable<Variable> scopeVars, SortedDictionary<(string op, int size), Function> bvOps) {
      if (vc == VCExpressionGenerator.True) {
        return new LiteralExpr(Token.NoToken, true);
      } else if (vc == VCExpressionGenerator.False) {
        return new LiteralExpr(Token.NoToken, false);
      } else if (vc is VCExprIntLit) {
        return new LiteralExpr(Token.NoToken, ((VCExprIntLit)vc).Val);
      } else if (vc is VCExprRealLit) {
        return new LiteralExpr(Token.NoToken, ((VCExprRealLit)vc).Val);
      } else if (vc is VCExprNAry) {
        VCExprNAry nary = vc as VCExprNAry;
        VCExprOp op = nary.Op;
        if (nary.Arity == 1) {
          if (op is VCExprBvOp) {
            return new LiteralExpr(Token.NoToken, ((VCExprIntLit)nary[0]).Val, ((VCExprBvOp)op).Bits);
          }

          Expr arg0 = VCtoExpr(nary[0], scopeVars, bvOps);
          if (op == VCExpressionGenerator.NotOp) {
            return Expr.Not(arg0);
          } else if (op == VCExpressionGenerator.ToIntOp) {
            return new NAryExpr(Token.NoToken, new ArithmeticCoercion(Token.NoToken, ArithmeticCoercion.CoercionType.ToInt), new List<Expr> { arg0 });
          } else if (op == VCExpressionGenerator.ToRealOp) {
            return new NAryExpr(Token.NoToken, new ArithmeticCoercion(Token.NoToken, ArithmeticCoercion.CoercionType.ToReal), new List<Expr> { arg0 });
          } else if (op is VCExprBoogieFunctionOp) {
            VCExprBoogieFunctionOp fnOp = op as VCExprBoogieFunctionOp;
            Function fn = fnOp.Func;
            return new NAryExpr(Token.NoToken, new FunctionCall(fn), new List<Expr> { arg0 });
          } else if (op is VCExprBvExtractOp) {
            VCExprBvExtractOp extractOp = op as VCExprBvExtractOp;
            return new BvExtractExpr(Token.NoToken, arg0, extractOp.End, extractOp.Start);
           }
        } else if (nary.Arity == 2) {
          Expr arg0 = VCtoExpr(nary[0], scopeVars, bvOps);
          Expr arg1 = VCtoExpr(nary[1], scopeVars, bvOps);
          if (op == VCExpressionGenerator.AddIOp || op == VCExpressionGenerator.AddROp) {
            return Expr.Add(arg0, arg1);
          } else if (op == VCExpressionGenerator.SubIOp || op == VCExpressionGenerator.SubROp) {
            return Expr.Sub(arg0, arg1);
          } else if (op == VCExpressionGenerator.MulIOp || op == VCExpressionGenerator.MulROp) {
            return Expr.Mul(arg0, arg1);
          } else if (op == VCExpressionGenerator.DivIOp) {
            return Expr.Div(arg0, arg1);
          } else if (op == VCExpressionGenerator.DivROp) {
            return Expr.RealDiv(arg0, arg1);
          } else if (op == VCExpressionGenerator.ModOp) {
            return Expr.Mod(arg0, arg1);
          } else if (op == VCExpressionGenerator.EqOp) {
            return Expr.Eq(arg0, arg1);
          } else if (op == VCExpressionGenerator.GtOp) {
            return Expr.Gt(arg0, arg1);
          } else if (op == VCExpressionGenerator.LtOp) {
            return Expr.Lt(arg0, arg1);
          } else if (op == VCExpressionGenerator.LeOp) {
            return Expr.Le(arg0, arg1);
          } else if (op == VCExpressionGenerator.GeOp) {
            return Expr.Ge(arg0, arg1);
          } else if (op == VCExpressionGenerator.AndOp) {
            return Expr.And(arg0, arg1);
          } else if (op == VCExpressionGenerator.OrOp) {
            return Expr.Or(arg0, arg1);
          } else if (op == VCExpressionGenerator.ImpliesOp) {
            return Expr.Imp(arg0, arg1);
          } else if (op is VCExprBoogieFunctionOp) {
            VCExprBoogieFunctionOp fnOp = op as VCExprBoogieFunctionOp;
            return new NAryExpr(Token.NoToken, new FunctionCall(fnOp.Func), new List<Expr> { arg0, arg1 });
          } 
        } else if (nary.Arity == 3) {
          Expr arg0 = VCtoExpr(nary[0], scopeVars, bvOps);
          Expr arg1 = VCtoExpr(nary[1], scopeVars, bvOps);
          Expr arg2 = VCtoExpr(nary[2], scopeVars, bvOps);
          if (op == VCExpressionGenerator.IfThenElseOp) {
            return new NAryExpr(Token.NoToken, new IfThenElse(Token.NoToken), new List<Expr> { arg0, arg1, arg2 });
          }
        }
      } else if (vc is VCExprVar) {
        VCExprVar vcVar = vc as VCExprVar;
        foreach (Variable v in scopeVars) {
          // this might be a little naive - variables can be distinct yet share a name - but shouldn't be an issue here?
          if (vcVar.Name == v.Name) {
            IdentifierExpr i = new IdentifierExpr(Token.NoToken, vcVar.Name);
            i.Decl = v;
            return i;
          }
        }
      } else if (vc is VCExprLet) {
        VCExprLet vcLet = vc as VCExprLet;
        List<Variable> dummies = new List<Variable>();
        List<Expr> rhss = new List<Expr>();
        for (int i = 0; i < vcLet.Count(); i++) {
          VCExprLetBinding binding = vcLet[i];
          dummies.Add(new BoundVariable(Token.NoToken, new TypedIdent(Token.NoToken, binding.V.Name, binding.V.Type)));
          rhss.Add(VCtoExpr(binding.E, scopeVars, bvOps));
        }

        IEnumerable<Variable> scopeVarsWithBound = scopeVars.Concat(dummies);
        Expr body = VCtoExpr(vcLet.Body, scopeVarsWithBound, bvOps);

        return new LetExpr(Token.NoToken, dummies, rhss, null, body);
      } else if (vc is VCExprQuantifier) {
        VCExprQuantifier vcQuant = vc as VCExprQuantifier;
        List<Variable> dummies = new List<Variable>();
        foreach (VCExprVar bound in vcQuant.BoundVars) {
          dummies.Add(new BoundVariable(Token.NoToken, new TypedIdent(Token.NoToken, bound.Name, bound.Type)));
        }

        IEnumerable<Variable> scopeVarsWithBound = scopeVars.Concat(dummies);
        Expr body = VCtoExpr(vcQuant.Body, scopeVarsWithBound, bvOps);
        if (vcQuant.Quan == Quantifier.ALL) {
          return new ForallExpr(Token.NoToken, dummies, body);
        } else {
          return new ExistsExpr(Token.NoToken, dummies, body);
        }
      }

      throw new NotImplementedException("unimplemented for conversion to Expr: " + vc);
    }
    private static Dictionary<Procedure, Implementation[]> ComputeProcImplMap(Program program) {
      Contract.Requires(program != null);
      // Since implementations call procedures (impl. signatures) 
      // rather than directly calling other implementations, we first
      // need to compute which implementations implement which
      // procedures and remember which implementations call which
      // procedures.

      return program
      .Implementations
      .GroupBy(i => i.Proc).Select(g => g.ToArray()).ToDictionary(a => a[0].Proc);
    }

    class ExprContainsFunc : ReadOnlyVisitor {
      private string name;
      private bool contained = false;

      public static bool Check(Expr expr, String name) {
        ExprContainsFunc checker = new ExprContainsFunc(name);
        checker.Visit(expr);
        return checker.contained;
      }

      public ExprContainsFunc(String name) {
        this.name = name;
      }
      public override Expr VisitNAryExpr(NAryExpr node) {
        if (node.Fun is FunctionCall) {
          if (node.Fun.FunctionName == name) {
            contained = true;
          }
          return node;
        } else {
          this.VisitExprSeq(node.Args);
          return node;
        }
      }
    }

    class ForallCollector : ReadOnlyVisitor {
      public List<ForallExpr> foralls;

      public ForallCollector() {
        this.foralls = new List<ForallExpr>();
      }

      public override Expr VisitForallExpr(ForallExpr node) {
        foralls.Add(node);
        return base.VisitForallExpr(node);
      }
    }

  }
  public class InvariantInferrer {

    private Boogie2VCExprTranslator translator;
    private VCExpressionGenerator gen;
    private ProverInterface prover;
    private InterpolationProver interpol;
    //private IEnumerable<Variable> scopeVars;
    private Implementation impl;
    private Block loopHead;
    private SortedDictionary<(string op, int size), Function> bvOps;
    private List<Function> newBVFunctions;
    private bool aggressiveQE = CommandLineOptions.Clo.AggressiveQE;
    private bool manualQE = CommandLineOptions.Clo.ManualQE;
    private Dictionary<Function, VCExpr> functionDefs;
    private List<VCExpr> QFAxioms;

    public InvariantInferrer(Program program, Dictionary<Procedure, Implementation[]> procImpl, Implementation impl, Block loopHead,
      ProverInterface prover, InterpolationProver interpol, SortedDictionary<(string, int), Function> bvOps, List<Function> newBVFunctions, Dictionary<Function, VCExpr> functionDefs, List<VCExpr> QFAxioms) {
      this.translator = prover.Context.BoogieExprTranslator;
      this.gen = prover.Context.ExprGen;
      //this.scopeVars = scopeVars;
      this.prover = prover;
      this.impl = impl;
      this.loopHead = loopHead;
      this.interpol = interpol;
      this.bvOps = bvOps;
      this.newBVFunctions = newBVFunctions;
      this.functionDefs = functionDefs;
      this.QFAxioms = QFAxioms;

    }
    public VCExpr InferLoopInvariant() {
      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();
      Stopwatch loopTime = new Stopwatch();

      Block start = impl.Blocks[0];
      List<Block> ends = getEndBlocks();

      VCExpr requires = convertRequires();
      VCExpr ensures = convertEnsures();

      VCExpr loopP = getLoopPreCondition(requires, start);
      VCExpr loopQ = getLoopPostCondition(ensures, ends);

      VCExpr loopPElim = prover.EliminateQuantifiers(loopP, bvOps, newBVFunctions);
      VCExpr loopQElim = prover.EliminateQuantifiers(loopQ, bvOps, newBVFunctions);
      /*
      VCExpr loopPElim = loopP;
      VCExpr loopQElim = loopQ;

      if (CommandLineOptions.Clo.InterpolantSolverKind != CommandLineOptions.InterpolantSolver.Princess) {
        loopPElim = prover.EliminateQuantifiers(loopP, scopeVars, bvOps, newBVFunctions);
        loopQElim = prover.EliminateQuantifiers(loopQ, scopeVars, bvOps, newBVFunctions);
      }
      */

      // probably can improve this
      List<List<Block>> loopPaths = new List<List<Block>>();
      DoDFSVisitLoop(loopHead, loopPaths);
      HashSet<Block> loopBody = new HashSet<Block>();
      foreach (List<Block> l in loopPaths) {
        foreach (Block b in l) {
          loopBody.Add(b);
        }
      }
      if (loopBody.Count == 0) {
        loopBody.Add(loopHead);
      }

      // squeezing algorithm
      
      VCExpr K = getLoopGuard(loopBody); // guard, now only used for induction, hopefully can be removed there too because it's annoying

      List<VCExpr> A = new List<VCExpr> { loopPElim }; //A_0 = P
      List<VCExpr> B = new List<VCExpr> { gen.NotSimp(loopQElim) };
      //List<VCExpr> B = new List<VCExpr> { gen.AndSimp(NotK, gen.NotSimp(loopQElim)) }; //B_0 = !K && !Q

      bool Forward = CommandLineOptions.Clo.ForwardSqueeze;
      CommandLineOptions.InterpolationDebug DebugLevel = CommandLineOptions.Clo.InterpolationDebugLevel;
      int t = 0;
      int r = 0;
      int backtracks = 0;
      int interpolantiterations = 0;
      int concrete = 0;
      int iterations = 0;
      bool doConcrete = false;

      while (true) {
        if (CommandLineOptions.Clo.InterpolationProfiling) {
          if (iterations > 0) {
            loopTime.Stop();
            Console.WriteLine("iter " + iterations + " time: " + String.Format("{0:N3}", stopWatch.Elapsed.TotalSeconds));
            loopTime.Reset();
            loopTime.Start();
          } else {
            loopTime.Start();
          }
        }

        iterations++;

        VCExpr ADisjunct = listDisjunction(A);
        VCExpr B_rElim = B[r];
        //if (CommandLineOptions.Clo.InterpolantSolverKind != CommandLineOptions.InterpolantSolver.Princess) {

        if (!aggressiveQE) {
          B_rElim = prover.EliminateQuantifiers(B[r], bvOps, newBVFunctions);
          
          if (B[r] == B_rElim) {
            B_rElim = prover.Simplify(B_rElim, bvOps, newBVFunctions);
          }
          B[r] = B_rElim;
        }
          //}

        if (DebugLevel == CommandLineOptions.InterpolationDebug.All) {
          Console.WriteLine("B: " + B_rElim);
          Console.Out.Flush();
        }

        if (DebugLevel == CommandLineOptions.InterpolationDebug.All || DebugLevel == CommandLineOptions.InterpolationDebug.SizeOnly) {
          int Asize = SizeComputingVisitor.ComputeSize(A[t]);
          int Bsize = SizeComputingVisitor.ComputeSize(B[r]);
          int ADsize = SizeComputingVisitor.ComputeSize(ADisjunct);
          Console.WriteLine("iteration " + iterations + " has A_disjunct size " + ADsize + ", A size " + Asize + " and B size " + Bsize);
          Console.Out.Flush();
        }

        VCExpr I;
        if (interpol.CalculateInterpolant(B_rElim, ADisjunct, Forward, out I, bvOps, newBVFunctions, functionDefs, QFAxioms)) { 
          interpolantiterations++;

          VCExpr InvariantCandidate;
          if (Forward) {
            InvariantCandidate = I;
          } else {
            InvariantCandidate = gen.NotSimp(I);
          }
          // only matters for princess sometimes
          InvariantCandidate = prover.EliminateQuantifiers(InvariantCandidate, bvOps, newBVFunctions);
          /*
          if (DebugLevel == CommandLineOptions.InterpolationDebug.All) {
            Console.WriteLine("invar candidate: " + InvariantCandidate.ToString());
            Console.Out.Flush();
          }*/

          if (DebugLevel == CommandLineOptions.InterpolationDebug.All
            || DebugLevel == CommandLineOptions.InterpolationDebug.SizeOnly) {
            int size = SizeComputingVisitor.ComputeSize(InvariantCandidate);
            //Console.WriteLine("invar candidate raw: " + I.ToString());
            Console.WriteLine("invar candidate: " + InvariantCandidate.ToString());
            Console.WriteLine("iteration " + iterations + " has interpolant size " + size);
            Console.Out.Flush();

            // extra checks to catch potential unsoundness in algorithm, only do when debug info enabled
            /*
            if (!satisfiable(gen.ImpliesSimp(gen.AndSimp(InvariantCandidate, NotK), loopQ))) {
              Console.WriteLine("generated invariant doesn't satisfy I & !K ==> Q, after " + iterations + " iterations, including " + concrete + " concrete steps");
              Console.Out.Flush();
              throw new Exception("generated invariant doesn't satisfy I & !K ==> Q, after " + iterations + " iterations, including " + concrete + " concrete steps");
            }
            if (!satisfiable(gen.ImpliesSimp(loopP, InvariantCandidate))) {
              Console.WriteLine("generated invariant doesn't satisfy P ==> I, after " + iterations + " iterations, including " + concrete + " concrete steps");
              Console.Out.Flush();
              throw new Exception("generated invariant doesn't satisfy P ==> I, after " + iterations + "iterations, including " + concrete + " concrete steps");
            }
            */
          }
          
          if (isInductive(InvariantCandidate, loopBody, K)) {
            /* 
             * this catches some potential unsoundness
            if (satisfiable(gen.NotSimp(gen.AndSimp(notI, NotK)), prover)) {
              Console.WriteLine("invariant is guard or weaker version of it, after " + iterations + "iterations, including " + concrete + " concrete steps");
              Console.Out.Flush();
              throw new Exception("invariant is guard or weaker version of it, after " + iterations + "iterations, including " + concrete + " concrete steps");
            }
            */
            if (DebugLevel != CommandLineOptions.InterpolationDebug.Stats) {
              Console.WriteLine("invariant found after " + iterations + " iterations, " + " including " + concrete + " concrete steps");
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            
            if (DebugLevel == CommandLineOptions.InterpolationDebug.All || DebugLevel == CommandLineOptions.InterpolationDebug.Stats) {
              String seconds = String.Format("{0:N3}", ts.TotalSeconds);
              // success, iterations, abstract iterations, concrete iterations, invariant size, time taken in seconds
              Console.WriteLine("success," + iterations + "," + interpolantiterations + "," + concrete + "," + SizeComputingVisitor.ComputeSize(InvariantCandidate) + "," + seconds);
              Console.Out.Flush();
            }
            return InvariantCandidate; // found invariant
          }



          // extra check - sometimes algorithm gives invariant candidate !I such that P && !I is inductive but !I isn't inductive, if this is the case, just using !I as invariant is good enough for Boogie
          /*
          if (isInductive(gen.AndSimp(loopPElim, notI), loopHead, loopBody, K, prover, scopeVars)) {
            Console.WriteLine("invariant found after " + iterations + " iterations, " + " including " + concrete + " concrete steps");
            return notI; // found invariant
          }
          */

          VCExpr AExpr;
          if (Forward) {
            AExpr = setSP(loopHead, loopHead, I, loopBody);
          } else {
            AExpr = setSP(loopHead, loopHead, A[t], loopBody);
            //AExpr = setSP(loopHead, loopHead, gen.AndSimp(A[t], K), loopBody);
          }
          VCExpr AElim = AExpr;
          //if (CommandLineOptions.Clo.InterpolantSolverKind == CommandLineOptions.InterpolantSolver.Princess) {
          //   AElim = interpol.EliminateQuantifiers(AExpr, scopeVars, bvOps, newBVFunctions);
          // } else {
          if (!aggressiveQE) {
            AElim = prover.EliminateQuantifiers(AExpr, bvOps, newBVFunctions);
            if (AExpr == AElim) {
              AElim = prover.Simplify(AElim, bvOps, newBVFunctions);
            }
          }
          // }
          if (DebugLevel == CommandLineOptions.InterpolationDebug.All) {
            Console.WriteLine("A: " + AElim);
            Console.Out.Flush();
          }
          A.Insert(t + 1, AElim);
          t++;


          if (Forward) {
            B.Insert(r + 1, gen.OrSimp(B[0], gen.NotSimp(setWP(loopHead, loopHead, gen.NotSimp(B[r]), loopBody))));
          } else {
            B.Insert(r + 1, gen.OrSimp(I, gen.NotSimp(setWP(loopHead, loopHead, InvariantCandidate, loopBody))));
            //B.Insert(r + 1, gen.OrSimp(I, gen.AndSimp(K, setWP(loopHead, loopHead, I, loopBody))));
          }
          r++;
        } else {
          if ((Forward && t <= concrete) || (!Forward && r <= concrete)) {
            if (DebugLevel != CommandLineOptions.InterpolationDebug.Stats) {
              Console.WriteLine("failed after " + iterations + " iterations");
              Console.Out.Flush();
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            if (DebugLevel == CommandLineOptions.InterpolationDebug.All || DebugLevel == CommandLineOptions.InterpolationDebug.Stats) {
              String seconds = String.Format("{0:N3}", ts.TotalSeconds);
              // success, iterations, abstract iterations, concrete iterations, invariant size, time taken in seconds
              Console.WriteLine("failure," + iterations + "," + interpolantiterations + "," + concrete + ",," + seconds);
              Console.Out.Flush();
            }
            return VCExpressionGenerator.True; // fail to find invariant
          } else {
            backtracks++; 
            if (Forward) {
              t = concrete;
              VCExpr AExpr = setSP(loopHead, loopHead, A[t], loopBody);
              VCExpr AElim = AExpr;
              // if (CommandLineOptions.Clo.InterpolantSolverKind == CommandLineOptions.InterpolantSolver.Princess) {
              //   AElim = interpol.EliminateQuantifiers(AExpr, scopeVars, bvOps, newBVFunctions);
              //  } else {
              if (!aggressiveQE) {
                AElim = prover.EliminateQuantifiers(AExpr, bvOps, newBVFunctions);
                if (AExpr == AElim) {
                  AElim = prover.Simplify(AElim, bvOps, newBVFunctions);
                }
              }
              //   }

              A.Insert(t + 1, AElim);
              t++;
            } else {
              r = concrete;
              B.Insert(r + 1, gen.OrSimp(B[0], gen.NotSimp(setWP(loopHead, loopHead, gen.NotSimp(B[r]), loopBody))));
              //B.Insert(r + 1, gen.OrSimp(B[0], gen.AndSimp(K, setWP(loopHead, loopHead, B[r], loopBody))));
              r++;
            }
            concrete++;
          }
          doConcrete = false;
        }
      }
      Console.WriteLine("gave up on finding invariant after " + iterations + " iterations, " + " including " + concrete + " concrete steps");
      stopWatch.Stop();
      return VCExpressionGenerator.True;
    }

    // naive implementation, will surely need to make more sophisticated, just expects loop guard to be in assume statement at start of second block of any loop path
    // works in cases transformed directly from while loop, but might have issues with assembly-level unstructured code?
    private VCExpr getLoopGuard(HashSet<Block> loopBody) {
      List<VCExpr> loopGuards = new List<VCExpr>();
      foreach (Block nextBlock in loopHead.Exits()) {
        if (loopBody.Contains(nextBlock)) {
          List<VCExpr> blockGuards = new List<VCExpr>();
          foreach (Cmd c in nextBlock.Cmds) {
            AssumeCmd a = c as AssumeCmd;
            if (a != null) {
              blockGuards.Add(translator.Translate(a.Expr));
            } else {
              break;
            }
          }
          VCExpr blockGuard = VCExpressionGenerator.True;
          foreach (VCExpr g in blockGuards) {
            blockGuard = gen.AndSimp(blockGuard, g);
          }
          if (blockGuard != VCExpressionGenerator.True) {
            loopGuards.Add(blockGuard);
          }
        }
      }
      VCExpr loopGuard = VCExpressionGenerator.False;
      foreach (VCExpr g in loopGuards) {
        loopGuard = gen.OrSimp(loopGuard, g);
      }
      if (loopGuards.Count == 0) {
        loopGuard = VCExpressionGenerator.True;
      }
      return loopGuard;
    }

    private bool satisfiable(VCExpr predicate) {
      prover.BeginCheck("invariant inference check", predicate, null); // as far as I can tell, the ErrorHandler parameter here is not used at all and we don't want to use it anyway
      ProverInterface.Outcome outcome = prover.CheckOutcomeBasic();
      switch (outcome) {
        case (ProverInterface.Outcome.Valid):
          return true;
        case (ProverInterface.Outcome.Invalid):
          return false;
        default:
          throw new ProverException(outcome.ToString());
      }
    }

    private bool isInductive(VCExpr invarCandidate, HashSet<Block> loopBody, VCExpr guard) {
      VCExpressionGenerator gen = prover.Context.ExprGen;
      VCExpr loopBodyWP = setWP(loopHead, loopHead, invarCandidate, loopBody);
      VCExpr imp = gen.ImpliesSimp(gen.AndSimp(invarCandidate, guard), loopBodyWP); 
      //VCExpr loopBodySP = setSP(loopHead, loopHead, gen.AndSimp(invarCandidate, guard), loopBody, gen, translator);
      //VCExpr imp = gen.ImpliesSimp(loopBodySP, invarCandidate);
      return satisfiable(imp);
    }

    private VCExpr listDisjunction(List<VCExpr> list) {
      VCExpr disjunction = list[0];
      for (int i = 1; i < list.Count; i++) {
        disjunction = gen.OrSimp(disjunction, list[i]);
      }
      return disjunction;
    }

    private VCExpr convertRequires() {
      Substitution formalProcImplSubst = Substituter.SubstitutionFromDictionary(impl.GetImplFormalMap());
      // default requires is true
      VCExpr requires = VCExpressionGenerator.True;
      foreach (Requires req in impl.Proc.Requires) {
        Expr e = Substituter.Apply(formalProcImplSubst, req.Condition);
        VCExpr pred = translator.Translate(e);
        requires = gen.AndSimp(requires, pred);
      }
      return requires;
    }

    private VCExpr convertEnsures() {
      Substitution formalProcImplSubst = Substituter.SubstitutionFromDictionary(impl.GetImplFormalMap());
      // default ensures is true
      VCExpr ensures = VCExpressionGenerator.True;
      foreach (Ensures ens in impl.Proc.Ensures) {
        Expr e = Substituter.Apply(formalProcImplSubst, ens.Condition);
        VCExpr pred = translator.Translate(e);
        ensures = gen.AndSimp(ensures, pred);
      }
      return ensures;
    }

    private List<Block> getEndBlocks() {
      List<Block> ends = new List<Block>();
      foreach (Block b in impl.Blocks) {
        if (b.TransferCmd is ReturnCmd) {
          ends.Add(b);
        }
      }
      return ends;
    }
    
    private void DoDFSVisit(Block block, Block target, List<List<Block>> paths) {

      // case 2. We visit a node that ends with a return => path does not reach target
      if (block.TransferCmd is ReturnCmd) {
        paths.Remove(paths.Last());
        return;
      }

      // case 3. We visit a node with successors => continue the exploration of its successors
      bool firstSuccessor = false;
      List<Block> branchPoint = paths.Last().ToList();
      foreach (Block nextBlock in block.Exits()) {
        if (nextBlock != target) {
          if (!firstSuccessor) {
            firstSuccessor = true;
          } else {
            // create new path for new branch
            paths.Add(branchPoint.ToList());
          }
          paths.Last().Add(nextBlock);
          DoDFSVisit(nextBlock, target, paths);
        }
      }
    }

    private void DoDFSVisitLoop(Block block, List<List<Block>> paths) {

      // case 1. We visit the loop head
      if (block == loopHead && !paths.Any()) {
        // initial case
        paths.Add(new List<Block> { loopHead });
      }

      // case 2. We visit a node that ends with a return => path does not reach target
      if (block.TransferCmd is ReturnCmd) {
        paths.Remove(paths.Last());
        return;
      }

      // case 3. We visit a node with successors => continue the exploration of its successors
      bool firstSuccessor = false;
      List<Block> branchPoint = paths.Last().ToList();
      foreach (Block nextBlock in block.Exits()) {
        if (nextBlock != loopHead) {
          if (!firstSuccessor) {
            firstSuccessor = true;
          } else {
            // create new path for new branch
            paths.Add(branchPoint.ToList());
          }
          paths.Last().Add(nextBlock); // don't want target in path
          DoDFSVisitLoop(nextBlock, paths);
        }
      }
    }

    private void DoBackwardsDFSVisit(Block block, Block target, List<List<Block>> paths) {

      // case 2. We visit a node that has no predecessors => path does not reach loop
      if (block.Predecessors.Count == 0) {
        paths.Remove(paths.Last());
        return;
      }

      // case 3. We visit a node with predecessors => continue the exploration of its predecessors
      if (block.Predecessors != null) {
        bool firstPredecessor = false;
        List<Block> branchPoint = paths.Last().ToList();
        foreach (Block nextBlock in block.Predecessors) {
          if (nextBlock != target) {
            if (!firstPredecessor) {
              firstPredecessor = true;
            } else {
              // create new path for new branch
              paths.Add(branchPoint.ToList());
            }
            paths.Last().Add(nextBlock);
            DoBackwardsDFSVisit(nextBlock, target, paths);
          }
        }
      }
    }


    private VCExpr setWP(Block initial, Block target, VCExpr Q_In, HashSet<Block> blocks) {
      return setWP(new List<Block> { initial }, target, Q_In, blocks);
    }
    private VCExpr setWP(List<Block> initials, Block target, VCExpr Q_In, HashSet<Block> blocks) {
      Debug.Assert(blocks.Count > 0);

      Dictionary<Block, VCExpr> blockWPs = new Dictionary<Block, VCExpr>();
      Queue<Block> toTry = new Queue<Block>();
      foreach (Block initial in initials) {
        blockWPs.Add(initial, blockWP(initial, Q_In));
        foreach (Block predecessor in initial.Predecessors) {
          if (blocks.Contains(predecessor)) {
            toTry.Enqueue(predecessor);
          }
        }
        if (blocks.Count == 1) {
          return blockWPs[initial];
        }
      }

      while (toTry.Count != 0) {
        Block currentBlock = toTry.Dequeue();
        VCExpr blockQ_In = VCExpressionGenerator.True;
        bool successorsDone = true;
        foreach (Block successor in currentBlock.Exits()) {
          if (blocks.Contains(successor) && currentBlock != successor) {
            if (blockWPs.ContainsKey(successor)) {
              blockQ_In = gen.AndSimp(blockWPs[successor], blockQ_In);
            } else {
              successorsDone = false;
              break;
            }
          }
        }
        if (!successorsDone) {
          toTry.Enqueue(currentBlock);
          continue;
        }
        if (currentBlock == target) {
          return blockQ_In;
        }

        if (blockWPs.ContainsKey(currentBlock)) {
          continue;
        }

        blockWPs.Add(currentBlock, blockWP(currentBlock, blockQ_In));
        foreach (Block predecessor in currentBlock.Predecessors) {
          if (blocks.Contains(predecessor) && (!blockWPs.ContainsKey(predecessor) || predecessor == target)) {
            toTry.Enqueue(predecessor);
          }
        }
      }
      throw new cce.UnreachableException();
    }

    private VCExpr setSP(Block initial, Block target, VCExpr P_In, HashSet<Block> blocks) {
      Debug.Assert(blocks.Count > 0);

      Dictionary<Block, VCExpr> blockSPs = new Dictionary<Block, VCExpr>();
      Queue<Block> toTry = new Queue<Block>();
      blockSPs.Add(initial, blockSP(initial, P_In));

      foreach (Block successor in initial.Exits()) {
        if (blocks.Contains(successor)) {
          toTry.Enqueue(successor);
        }
      }

      if (blocks.Count == 1) {
        return blockSPs[initial];
      }

      while (toTry.Count != 0) {
        Block currentBlock = toTry.Dequeue();
        VCExpr blockP_In = VCExpressionGenerator.False;
        bool predecessorsDone = true;
        if (currentBlock.Predecessors.Count == 0) {
          blockP_In = VCExpressionGenerator.True;
        }
        foreach (Block predecessor in currentBlock.Predecessors) {
          if (blocks.Contains(predecessor) && currentBlock != predecessor) {
            if (blockSPs.ContainsKey(predecessor)) {
              blockP_In = gen.OrSimp(blockSPs[predecessor], blockP_In);
            } else {
              predecessorsDone = false;
              break;
            }
          }
        }
        if (!predecessorsDone) {
          toTry.Enqueue(currentBlock);
          continue;
        }
        if (currentBlock == target) {
          return blockP_In;
        }

        if (blockSPs.ContainsKey(currentBlock)) {
          continue;
        }

        blockSPs.Add(currentBlock, blockSP(currentBlock, blockP_In));
        foreach (Block successor in currentBlock.Exits()) {
          if (blocks.Contains(successor) && (!blockSPs.ContainsKey(successor) || successor == target)) {
            toTry.Enqueue(successor);
          }
        }
      }
      throw new cce.UnreachableException();
    }

    private VCExpr blockWP(Block block, VCExpr Q_in) {
      VCExpr Q = Q_in;
      for (int i = block.Cmds.Count; --i >= 0;) {
        Q = weakestPrecondition(block.Cmds[i], Q);
      }
      return Q;
    }

    private VCExpr blockSP(Block block, VCExpr P_in) {
      VCExpr P = P_in;
      foreach (Cmd c in block.Cmds) {
        P = strongestPostcondition(c, P);
      }
      return P;
    }

    private VCExpr getLoopPostCondition(VCExpr ensures, List<Block> ends) {
      HashSet<Block> blocks = new HashSet<Block>();
      foreach (Block end in ends) {
        List<List<Block>> paths = new List<List<Block>>();
        paths.Add(new List<Block> { end });
        DoBackwardsDFSVisit(end, loopHead, paths);

        foreach (List<Block> l in paths) {
          foreach (Block b in l) {
            blocks.Add(b);
          }
        }
      }
      blocks.Add(loopHead);

      return setWP(ends, loopHead, ensures, blocks);
    }

    private VCExpr getLoopPreCondition(VCExpr requires, Block start) {
      List<List<Block>> paths = new List<List<Block>>();
      paths.Add(new List<Block> { start });
      DoDFSVisit(start, loopHead, paths);
      HashSet<Block> blocks = new HashSet<Block>();
      foreach (List<Block> l in paths) {
        foreach (Block b in l) {
          blocks.Add(b);
        }
      }
      blocks.Add(loopHead);

      return setSP(start, loopHead, requires, blocks);
    }
    
    private VCExpr weakestPrecondition(Cmd cmd, VCExpr Q) {
      if (cmd is AssertCmd) {
        // assert A -> A && Q
        AssertCmd ac = (AssertCmd) cmd;
        VCExpr A = translator.Translate(ac.Expr);

        // handles edge case where loop has no postcondition - instead use last assertion as base case 
        /*
        if (Q == VCExpressionGenerator.False) {
          return gen.NotSimp(A);
        }
        */

        return gen.AndSimp(A, Q);
      } else if (cmd is AssumeCmd) {
        // assume A -> A ==> Q
        AssumeCmd ac = (AssumeCmd) cmd;
        VCExpr A = translator.Translate(ac.Expr);

        return gen.ImpliesSimp(A, Q);

      } else if (cmd is HavocCmd) {
        // havoc x -> forall x :: Q
        HavocCmd hc = (HavocCmd) cmd;
        List<VCExprVar> vars = new List<VCExprVar>();
        HashSet<VCExprVar> QFree = FreeVariableCollector.FreeTermVariables(Q);
        List<VCExpr> whereExprs = new List<VCExpr>();
        foreach (IdentifierExpr i in hc.Vars) {
          VCExprVar v = (VCExprVar)translator.Translate(i);
          if (QFree.Contains(v)) {
            vars.Add(v);
          }
          if (i.Decl.TypedIdent.WhereExpr != null) {
            whereExprs.Add(translator.Translate(i.Decl.TypedIdent.WhereExpr));
          }
        }

        if (whereExprs.Count > 0) {
          VCExpr where = VCExpressionGenerator.True;
          foreach (VCExpr w in whereExprs) {
            where = gen.AndSimp(where, w);
          }
          Q = gen.ImpliesSimp(where, Q);
        }

        if (vars.Count == 0) {
          return Q;
        } else {
          Dictionary<VCExprVar, VCExpr> toSubst = new Dictionary<VCExprVar, VCExpr>();
          List<VCExprVar> freshVars = new List<VCExprVar>();
          foreach (VCExprVar v in vars) {
            VCExprVar fresh = gen.Variable(v.Name + "'", v.Type); // need to make more sophisticated
            toSubst.Add(v, fresh);
            freshVars.Add(fresh);
          }

          VCExprSubstitution subst = new VCExprSubstitution(toSubst, new Dictionary<TypeVariable, Type>());
          SubstitutingVCExprVisitor substituter = new SubstitutingVCExprVisitor(gen);
          VCExpr substQ = substituter.Mutate(Q, subst);

          if (aggressiveQE) {
            return prover.EliminateQuantifiers(gen.Forall(freshVars, new List<VCTrigger>(), substQ), bvOps, newBVFunctions);
          } else {
            return gen.Forall(freshVars, new List<VCTrigger>(), substQ);
          }
        }
      } else if (cmd is AssignCmd) {
        // x := e -> Q[x\e]
        AssignCmd ac = (AssignCmd) cmd;
        ac = ac.AsSimpleAssignCmd;

        Dictionary<VCExprVar, VCExpr> assignments = new Dictionary<VCExprVar, VCExpr>();
        for (int i = 0; i < ac.Lhss.Count; ++i) {
          IdentifierExpr lhs = ((SimpleAssignLhs) ac.Lhss[i]).AssignedVariable;
          Expr rhs = ac.Rhss[i];
          VCExprVar lhsPred = (VCExprVar) translator.Translate(lhs);
          VCExpr rhsPred = translator.Translate(rhs);
          assignments.Add(lhsPred, rhsPred);
        }

        VCExprSubstitution subst = new VCExprSubstitution(assignments, new Dictionary<TypeVariable, Type>());
        SubstitutingVCExprVisitor substituter = new SubstitutingVCExprVisitor(gen);
        VCExpr substQ = substituter.Mutate(Q, subst);

        return substQ; 
      } else if (cmd is CallCmd) {
        CallCmd callCmd = (CallCmd)cmd;
        StateCmd desugar = callCmd.Desugaring as StateCmd;
        VCExpr QCall = Q;
        for (int i = desugar.Cmds.Count - 1; i >= 0; i--) {
          Cmd c = desugar.Cmds[i];
          QCall = weakestPrecondition(c, QCall);
        }
        return QCall;
      }
      throw new NotImplementedException("unimplemented command for WP: " + cmd.ToString());
    }

    private VCExpr strongestPostcondition(Cmd cmd, VCExpr P) {
      if (cmd is AssertCmd) {
        // assert A -> A && P
        AssertCmd ac = (AssertCmd)cmd;
        VCExpr A = translator.Translate(ac.Expr);

        return gen.AndSimp(A, P);
      } else if (cmd is AssumeCmd) {
        // assume A -> P && A
        AssumeCmd ac = (AssumeCmd)cmd;
        VCExpr A = translator.Translate(ac.Expr);

        return gen.AndSimp(P, A);

      } else if (cmd is HavocCmd) {
        // havoc x -> exists x' :: P[x\x']
        HavocCmd hc = (HavocCmd)cmd;
        List<VCExprVar> vars = new List<VCExprVar>();
        List<VCExpr> whereExprs = new List<VCExpr>();
        foreach (IdentifierExpr i in hc.Vars) {
          vars.Add((VCExprVar)translator.Translate(i));
          if (i.Decl.TypedIdent.WhereExpr != null) {
            whereExprs.Add(translator.Translate(i.Decl.TypedIdent.WhereExpr));
          }
        }
        Dictionary<VCExprVar, VCExpr> toSubst = new Dictionary<VCExprVar, VCExpr>();
        List<VCExprVar> freshVars = new List<VCExprVar>();

        HashSet<VCExprVar> PFree = FreeVariableCollector.FreeTermVariables(P);

        foreach (VCExprVar v in vars) {
          if (PFree.Contains(v)) {
            VCExprVar fresh = gen.Variable(v.Name + "'", v.Type); // need to make more sophisticated
            toSubst.Add(v, fresh);
            freshVars.Add(fresh);
          }
        }
        if (freshVars.Count == 0) {
          if (whereExprs.Count > 0) {
            foreach (VCExpr w in whereExprs) {
              P = gen.AndSimp(P, w);
            }
          }
          return P;
        } else {
          VCExprSubstitution subst = new VCExprSubstitution(toSubst, new Dictionary<TypeVariable, Type>());
          SubstitutingVCExprVisitor substituter = new SubstitutingVCExprVisitor(gen);
          VCExpr substP = substituter.Mutate(P, subst);

          //return substP;
          if (!manualQE) {
            substP = gen.Exists(freshVars, new List<VCTrigger>(), substP);
          }

          if (whereExprs.Count > 0) {
            foreach (VCExpr w in whereExprs) {
              substP = gen.AndSimp(substP, w);
            }
          }

          if (aggressiveQE) {
            return prover.EliminateQuantifiers(substP, bvOps, newBVFunctions);
          } else {
            return substP;
          }
        }

      } else if (cmd is AssignCmd) {
        // x := e -> exists x' :: P[x\x'] && x == e[x\x']
        AssignCmd ac = (AssignCmd)cmd;
        ac = ac.AsSimpleAssignCmd;

        List<(VCExprVar, VCExpr)> assignments = new List<(VCExprVar, VCExpr)>();
        Dictionary<VCExprVar, VCExpr> toSubst = new Dictionary<VCExprVar, VCExpr>();
        List<VCExprVar> freshVars = new List<VCExprVar>();
        HashSet<VCExprVar> PFree = FreeVariableCollector.FreeTermVariables(P);

        for (int i = 0; i < ac.Lhss.Count; ++i) {
          IdentifierExpr lhs = ((SimpleAssignLhs)ac.Lhss[i]).AssignedVariable;
          Expr rhs = ac.Rhss[i];
          VCExprVar lhsPred = (VCExprVar)translator.Translate(lhs);
          VCExpr rhsPred = translator.Translate(rhs);
          PFree.Union(FreeVariableCollector.FreeTermVariables(rhsPred));
          assignments.Add((lhsPred, rhsPred));
        }

        foreach ((VCExprVar, VCExpr) assign in assignments) {
          if (PFree.Contains(assign.Item1)) {
            VCExprVar fresh = gen.Variable(assign.Item1 + "'", assign.Item1.Type); // need to make more sophisticated
            toSubst.Add(assign.Item1, fresh);
            freshVars.Add(fresh);
          }
        }
        if (freshVars.Count == 0) {
          VCExpr sp = P;
          foreach ((VCExprVar, VCExpr) assign in assignments) {
            sp = gen.AndSimp(sp, gen.Eq(assign.Item1, assign.Item2));
          }
          return sp;
        } else {
          VCExprSubstitution subst = new VCExprSubstitution(toSubst, new Dictionary<TypeVariable, Type>());
          SubstitutingVCExprVisitor substituter = new SubstitutingVCExprVisitor(gen);
          VCExpr substP = substituter.Mutate(P, subst);

          foreach ((VCExprVar, VCExpr) assign in assignments) {
            VCExpr substRhs = substituter.Mutate(assign.Item2, subst);
            substP = gen.AndSimp(substP, gen.Eq(assign.Item1, substRhs));
          }

          if (manualQE) {
            return substP;
          }

          if (aggressiveQE) {
            return prover.EliminateQuantifiers(gen.Exists(freshVars, new List<VCTrigger>(), substP), bvOps, newBVFunctions);
          } else {
            return gen.Exists(freshVars, new List<VCTrigger>(), substP);
          }
        }

      } else if (cmd is CallCmd) {
        CallCmd callCmd = (CallCmd)cmd;
        StateCmd desugar = callCmd.Desugaring as StateCmd;
        VCExpr PCall = P;
        foreach (Cmd c in desugar.Cmds) {
          PCall = strongestPostcondition(c, PCall);
        }
        return PCall;
      }
      throw new NotImplementedException("unimplemented command for SP: " + cmd.ToString());
    }

  }
  
}
