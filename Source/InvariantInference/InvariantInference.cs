using System;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.Boogie.VCExprAST;
using Microsoft.Boogie.SMTLib;
using Microsoft.BaseTypes;

namespace Microsoft.Boogie.InvariantInference {
  public class InvariantInference {

    public static void RunInvariantInference(Program program) {
      // find back edges
      WidenPoints.Compute(program);
      Dictionary<Procedure, Implementation[]> procedureImplementations = ComputeProcImplMap(program);

      ProverInterface prover = ProverInterface.CreateProver(program, CommandLineOptions.Clo.ProverLogFilePath,
        CommandLineOptions.Clo.ProverLogFileAppend, CommandLineOptions.Clo.TimeLimit);

      InterpolationProver interpol = InterpolationProver.CreateProver(program, CommandLineOptions.Clo.InterpolationLogFilePath,
        CommandLineOptions.Clo.InterpolationLogFileAppend, CommandLineOptions.Clo.TimeLimit);

      // z3 apparently has a new interpolant algorithm - we should try it as well to compare

      // analyze each procedure
      foreach (var proc in program.Procedures) {
        if (procedureImplementations.ContainsKey(proc)) {
          // analyze each implementation of the procedure
          foreach (var impl in procedureImplementations[proc]) {
            // find back edge & check only one back edge per procedure implementation
            bool backEdgeFound = false;
            // naive attempt at getting variables that may be referred to
            IEnumerable<Variable> scopeVars = new List<Variable>();
            scopeVars = scopeVars.Concat(program.GlobalVariables);
            scopeVars = scopeVars.Concat(impl.LocVars);
            scopeVars = scopeVars.Concat(impl.InParams);
            scopeVars = scopeVars.Concat(impl.OutParams);
            foreach (Block b in impl.Blocks) {
              // add desugared variables, no idea if this is really needed?
              foreach (Cmd c in b.Cmds) {
                if (c is CallCmd) {
                  scopeVars = scopeVars.Concat(((StateCmd)((CallCmd)c).Desugaring).Locals);
                }
              }
            }
            foreach (Block b in impl.Blocks) {
              if (b.widenBlock) {
                if (backEdgeFound) {
                  // error
                  throw new NotImplementedException("cannot handle multiple back edges");
                } else {
                  backEdgeFound = true;

                  VCExpr invariant = InferLoopInvariant(program, procedureImplementations, impl, b, prover, interpol, scopeVars);
                  Instrument(b, invariant, scopeVars);
                }
              }
            }
          }
        }
      }
    }

    private static void Instrument(Block b, VCExpr invariant, IEnumerable<Variable> scopeVars) {
      // need to translate VCExpr back into Expr - kind of terrible
      Expr inv = VCtoExpr(invariant, scopeVars);
      List<Cmd> newCommands = new List<Cmd>();
      PredicateCmd cmd;
      var kv = new QKeyValue(Token.NoToken, "inferred", new List<object>(), null);
      if (CommandLineOptions.Clo.InstrumentWithAsserts) {
        cmd = new AssertCmd(Token.NoToken, inv, kv);
      } else {
        cmd = new AssumeCmd(Token.NoToken, inv, kv);
      }

      newCommands.Add(cmd);
      newCommands.AddRange(b.Cmds);
      b.Cmds = newCommands;
    }

    // more robust would be to use visitor
    // also need to take into account stack overflows potentially? 
    private static Expr VCtoExpr(VCExpr vc, IEnumerable<Variable> scopeVars) {
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
          Expr arg0 = VCtoExpr(nary[0], scopeVars);
          if (op == VCExpressionGenerator.NotOp) {
            return Expr.Not(arg0);
          } else if (op == VCExpressionGenerator.ToIntOp) {
            return new NAryExpr(Token.NoToken, new ArithmeticCoercion(Token.NoToken, ArithmeticCoercion.CoercionType.ToInt), new List<Expr> { arg0 });
          } else if (op == VCExpressionGenerator.ToRealOp) {
            return new NAryExpr(Token.NoToken, new ArithmeticCoercion(Token.NoToken, ArithmeticCoercion.CoercionType.ToReal), new List<Expr> { arg0 });
          }
        } else if (nary.Arity == 2) {
          Expr arg0 = VCtoExpr(nary[0], scopeVars);
          Expr arg1 = VCtoExpr(nary[1], scopeVars);
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
          }
        } else if (nary.Arity == 3) {
          Expr arg0 = VCtoExpr(nary[0], scopeVars);
          Expr arg1 = VCtoExpr(nary[1], scopeVars);
          Expr arg2 = VCtoExpr(nary[2], scopeVars);
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
          rhss.Add(VCtoExpr(binding.E, scopeVars));
        }

        IEnumerable<Variable> scopeVarsWithBound = scopeVars.Concat(dummies);
        Expr body = VCtoExpr(vcLet.Body, scopeVarsWithBound);

        return new LetExpr(Token.NoToken, dummies, rhss, null, body);
      }

      throw new NotImplementedException("unimplemented for conversion to Expr: " + vc);
    }

    private static VCExpr InferLoopInvariant(Program program, Dictionary<Procedure, Implementation[]> procImpl, Implementation impl, Block loopHead,
      ProverInterface prover, InterpolationProver interpol, IEnumerable<Variable> scopeVars) {
      Boogie2VCExprTranslator translator = prover.Context.BoogieExprTranslator;
      VCExpressionGenerator gen = prover.Context.ExprGen;

      Block start = impl.Blocks[0];
      List<Block> ends = getEndBlocks(impl);

      VCExpr requires = convertRequires(impl, gen, translator);
      VCExpr ensures = convertEnsures(impl, gen, translator);

      VCExpr loopP = getLoopPreCondition(requires, start, loopHead, gen, translator, prover, scopeVars);
      VCExpr loopQ = getLoopPostCondition(ensures, ends, loopHead, gen, translator, prover, scopeVars);

      VCExpr loopPElim = prover.EliminateQuantifiers(loopP, scopeVars);
      VCExpr loopQElim = prover.EliminateQuantifiers(loopQ, scopeVars);

      // probably can improve this
      List<List<Block>> loopPaths = new List<List<Block>>();
      DoDFSVisitLoop(loopHead, loopHead, loopPaths);
      HashSet<Block> loopBody = new HashSet<Block>();
      foreach (List<Block> l in loopPaths) {
        foreach (Block b in l) {
          loopBody.Add(b);
        }
      }

      // backward squeezing algorithm
      VCExpr K = getLoopGuard(loopHead, loopBody, gen, translator); // guard 
      List<VCExpr> A = new List<VCExpr> { loopPElim }; //A_0 = P
      List<VCExpr> B = new List<VCExpr> { gen.AndSimp(gen.NotSimp(K), gen.NotSimp(loopQElim)) }; //B_0 = !K && !Q
      int t = 0;
      int r = 0;
      int concrete = 0;
      int iterations = 0;
      bool ATooBig = false;
      bool doConcrete = false;
      string[] oldInterpolants = new string[4];
      int oldInterpolantIndex = 0;
      while (true) {
        iterations++;
        int Asize = SizeComputingVisitor.ComputeSize(A[t]);
        /*
        if (!ATooBig) {
          if (Asize > 10000) {
            ATooBig = true;
          }
        } */

        VCExpr ADisjunct = listDisjunction(A, gen);
        VCExpr B_rElim = prover.EliminateQuantifiers(B[r], scopeVars);
        int Bsize = SizeComputingVisitor.ComputeSize(B[r]);
        int ADsize = SizeComputingVisitor.ComputeSize(ADisjunct);
        Console.WriteLine("iteration " + iterations + " has A_disjunct size " + ADsize + ", A size " + Asize + " and B size " + Bsize);
        Console.Out.Flush();
        if (!doConcrete && !interpol.Satisfiable(B_rElim, ADisjunct)) {
          SExpr resp = interpol.CalculateInterpolant();
          /*
          OldOldInterpolant = OldInterpolant;
          OldInterpolant = interpolantStr;

          Console.WriteLine("interpolant: " + interpolantStr);
          if (interpolantStr == OldInterpolant && interpolantStr == OldOldInterpolant) {
            doConcrete = true;
            continue;
          } */
          VCExpr I = resp.ToVC(gen, translator, scopeVars, new Dictionary<String, VCExprVar>());
          VCExpr notI = gen.NotSimp(I);
          int size = SizeComputingVisitor.ComputeSize(notI);
          Console.WriteLine("invar candidate: " + notI.ToString());
          Console.WriteLine("iteration " + iterations + " has interpolant size " + size);
          Console.Out.Flush();

          if (!satisfiable(gen.ImpliesSimp(gen.AndSimp(notI, gen.NotSimp(K)), loopQ), prover)) {
            Console.WriteLine("generated invariant doesn't satisfy I & !K ==> Q, after " + iterations + "iterations, including " + concrete + " concrete steps");
            Console.Out.Flush();
            throw new Exception("generated invariant doesn't satisfy I & !K ==> Q, after " + iterations + "iterations, including " + concrete + " concrete steps");
          }
          if (!satisfiable(gen.ImpliesSimp(loopP, notI), prover)) {
            Console.WriteLine("generated invariant doesn't satisfy P ==> I, after " + iterations + "iterations, including " + concrete + " concrete steps");
            Console.Out.Flush();
            throw new Exception("generated invariant doesn't satisfy P ==> I, after " + iterations + "iterations, including " + concrete + " concrete steps");
          }

          if (isInductive(notI, loopHead, loopBody, K, prover, scopeVars)) {
            /*
            if (satisfiable(gen.NotSimp(gen.AndSimp(notI, gen.NotSimp(K))), prover)) {
              Console.WriteLine("invariant is guard or weaker version of it, after " + iterations + "iterations, including " + concrete + " concrete steps");
              Console.Out.Flush();
              throw new Exception("invariant is guard or weaker version of it, after " + iterations + "iterations, including " + concrete + " concrete steps");
            }
            */
            Console.WriteLine("invariant found after " + iterations + " iterations, " + " including " + concrete + " concrete steps");
            return notI; // found invariant
          }

          //if (!ATooBig) {
          VCExpr AExpr = setSP(loopHead, loopHead, gen.AndSimp(A[t], K), loopBody, gen, translator, prover, scopeVars);
          VCExpr AElim = prover.EliminateQuantifiers(AExpr, scopeVars);
          //Console.WriteLine("A: " + AElim);
          A.Insert(t + 1, AElim);
          t++;
          //}

          /*
          string interpolant = resp.ToString();
          int interpolantMatches = 0;
          for (int i = 0; i < 4; i++) {
            if (oldInterpolants[i] == interpolant) {
              interpolantMatches++;
            }
          }
          if (interpolantMatches >= 2) {
            doConcrete = true;
            for (int i = 0; i < 4; i++) {
              oldInterpolants[i] = null;
            }
          } else { 
            oldInterpolants[oldInterpolantIndex] = interpolant;
            oldInterpolantIndex++;
            if (oldInterpolantIndex > 3) {
              oldInterpolantIndex = 0;
            } */

            B.Insert(r + 1, gen.OrSimp(I, gen.AndSimp(K, setWP(loopHead, loopHead, I, loopBody, gen, translator, prover, scopeVars))));
            r++;
          //}
        } else {
          if (r <= concrete) {
            Console.WriteLine("failed after " + iterations + " iterations");
            return VCExpressionGenerator.True; // fail to find invariant
          } else {
            r = concrete;
            B.Insert(r + 1, gen.OrSimp(B[0], gen.AndSimp(K, setWP(loopHead, loopHead, B[r], loopBody, gen, translator, prover, scopeVars)))); 
            r++;
            concrete++;
          }
          doConcrete = false;
        }
      }
      Console.WriteLine("gave up on finding invariant after " + iterations + " iterations, " + " including " + concrete + " concrete steps");
      return VCExpressionGenerator.True;
    }

    // naive implementation, will surely need to make more sophisticated, just expects loop guard to be in assume statement at start of second block of any loop path
    // works in cases transformed directly from while loop, but might have issues with assembly-level unstructured code?
    private static VCExpr getLoopGuard(Block loopHead, HashSet<Block> loopBody, VCExpressionGenerator gen, Boogie2VCExprTranslator translator) {
      GotoCmd successors = (GotoCmd)loopHead.TransferCmd;
      if (successors.labelTargets != null) {
        foreach (Block nextBlock in successors.labelTargets) {
          if (loopBody.Contains(nextBlock)) {
            if (nextBlock.Cmds[0] is AssumeCmd) {
              AssumeCmd ac = (AssumeCmd)nextBlock.Cmds[0];
              return translator.Translate(ac.Expr);
            }
          }
        }
      }
      throw new cce.UnreachableException();
    }

    private static bool satisfiable(VCExpr predicate, ProverInterface prover) {
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

    private static bool isInductive(VCExpr invarCandidate, Block loopHead, HashSet<Block> loopBody, VCExpr guard, ProverInterface prover, IEnumerable<Variable> scopeVars) {
      Boogie2VCExprTranslator translator = prover.Context.BoogieExprTranslator;
      VCExpressionGenerator gen = prover.Context.ExprGen;
      VCExpr loopBodyWP = setWP(loopHead, loopHead, invarCandidate, loopBody, gen, translator, prover, scopeVars);
      VCExpr imp = gen.ImpliesSimp(gen.AndSimp(invarCandidate, guard), loopBodyWP);
      //VCExpr loopBodySP = setSP(loopHead, loopHead, gen.AndSimp(invarCandidate, guard), loopBody, gen, translator);
      //VCExpr imp = gen.ImpliesSimp(loopBodySP, invarCandidate);
      return satisfiable(imp, prover);
    }

    private static VCExpr listDisjunction(List<VCExpr> list, VCExpressionGenerator gen) {
      VCExpr disjunction = list[0];
      for (int i = 1; i < list.Count; i++) {
        disjunction = gen.OrSimp(disjunction, list[i]);
      }
      return disjunction;
    }

    private static VCExpr convertRequires(Implementation impl, VCExpressionGenerator gen, Boogie2VCExprTranslator translator) {
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

    private static VCExpr convertEnsures(Implementation impl, VCExpressionGenerator gen, Boogie2VCExprTranslator translator) {
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

    private static List<Block> getEndBlocks(Implementation impl) {
      List<Block> ends = new List<Block>();
      foreach (Block b in impl.Blocks) {
        if (b.TransferCmd is ReturnCmd) {
          ends.Add(b);
        }
      }
      return ends;
    }
    
    // need to completely rework paths approach
    private static void DoDFSVisit(Block block, Block target, List<List<Block>> paths) {

      // case 2. We visit a node that ends with a return => path does not reach target
      if (block.TransferCmd is ReturnCmd) {
        paths.Remove(paths.Last());
        return;
      }

      // case 3. We visit a node with successors => continue the exploration of its successors
      GotoCmd successors = (GotoCmd)block.TransferCmd;

      if (successors.labelTargets != null) {
        bool firstSuccessor = false;
        List<Block> branchPoint = paths.Last().ToList();
        foreach (Block nextBlock in successors.labelTargets) {
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
    }

    // need to fix case where loop is single block
    private static void DoDFSVisitLoop(Block block, Block loopHead, List<List<Block>> paths) {

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
      GotoCmd successors = (GotoCmd)block.TransferCmd;

      if (successors.labelTargets != null) { 
        bool firstSuccessor = false;
        List<Block> branchPoint = paths.Last().ToList();
        foreach (Block nextBlock in successors.labelTargets) {
          if (nextBlock != loopHead) {
            if (!firstSuccessor) {
              firstSuccessor = true;
            } else {
              // create new path for new branch
              paths.Add(branchPoint.ToList());
            }
            paths.Last().Add(nextBlock); // don't want target in path
            DoDFSVisitLoop(nextBlock, loopHead, paths);
          }
        }
      }
    }

    private static void DoBackwardsDFSVisit(Block block, Block target, List<List<Block>> paths) {

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


    private static VCExpr setWP(Block initial, Block target, VCExpr Q_In, HashSet<Block> blocks, VCExpressionGenerator gen, Boogie2VCExprTranslator translator, ProverInterface prover, IEnumerable<Variable> scopeVars) {
      return setWP(new List<Block> { initial }, target, Q_In, blocks, gen, translator, prover, scopeVars);
    }
    private static VCExpr setWP(List<Block> initials, Block target, VCExpr Q_In, HashSet<Block> blocks, VCExpressionGenerator gen, Boogie2VCExprTranslator translator, ProverInterface prover, IEnumerable<Variable> scopeVars) {
      Debug.Assert(blocks.Count > 0);

      Dictionary<Block, VCExpr> blockWPs = new Dictionary<Block, VCExpr>();
      Queue<Block> toTry = new Queue<Block>();
      foreach (Block initial in initials) {
        blockWPs.Add(initial, blockWP(initial, Q_In, gen, translator, prover, scopeVars));
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
        GotoCmd successors = currentBlock.TransferCmd as GotoCmd;
        VCExpr blockQ_In = VCExpressionGenerator.True;
        bool successorsDone = true;
        foreach (Block successor in successors.labelTargets) {
          if (blocks.Contains(successor)) {
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

        blockWPs.Add(currentBlock, blockWP(currentBlock, blockQ_In, gen, translator, prover, scopeVars));
        foreach (Block predecessor in currentBlock.Predecessors) {
          if (blocks.Contains(predecessor) && (!blockWPs.ContainsKey(predecessor) || predecessor == target)) {
            toTry.Enqueue(predecessor);
          }
        }
      }
      throw new cce.UnreachableException();
    }

    private static VCExpr setSP(Block initial, Block target, VCExpr P_In, HashSet<Block> blocks, VCExpressionGenerator gen, Boogie2VCExprTranslator translator, ProverInterface prover, IEnumerable<Variable> scopeVars) {
      Debug.Assert(blocks.Count > 0);

      Dictionary<Block, VCExpr> blockSPs = new Dictionary<Block, VCExpr>();
      Queue<Block> toTry = new Queue<Block>();
      blockSPs.Add(initial, blockSP(initial, P_In, gen, translator, prover, scopeVars));

      GotoCmd successors = initial.TransferCmd as GotoCmd;
      foreach (Block successor in successors.labelTargets) {
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
        foreach (Block predecessor in currentBlock.Predecessors) {
          if (blocks.Contains(predecessor)) {
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

        blockSPs.Add(currentBlock, blockSP(currentBlock, blockP_In, gen, translator, prover, scopeVars));
        successors = currentBlock.TransferCmd as GotoCmd;
        foreach (Block successor in successors.labelTargets) {
          if (blocks.Contains(successor) && (!blockSPs.ContainsKey(successor) || successor == target)) {
            toTry.Enqueue(successor);
          }
        }
      }
      throw new cce.UnreachableException();
    }

    private static VCExpr blockWP(Block block, VCExpr Q_in, VCExpressionGenerator gen, Boogie2VCExprTranslator translator, ProverInterface prover, IEnumerable<Variable> scopeVars) {
      VCExpr Q = Q_in;
      for (int i = block.Cmds.Count; --i >= 0;) {
        Q = weakestPrecondition(block.Cmds[i], Q, gen, translator, prover, scopeVars);
      }
      return Q;
    }

    private static VCExpr blockSP(Block block, VCExpr P_in, VCExpressionGenerator gen, Boogie2VCExprTranslator translator, ProverInterface prover, IEnumerable<Variable> scopeVars) {
      VCExpr P = P_in;
      foreach (Cmd c in block.Cmds) {
        P = strongestPostcondition(c, P, gen, translator, prover, scopeVars);
      }
      return P;
    }

    private static VCExpr getLoopPostCondition(VCExpr ensures, List<Block> ends, Block loopHead, VCExpressionGenerator gen, Boogie2VCExprTranslator translator, ProverInterface prover, IEnumerable<Variable> scopeVars) {
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

      return setWP(ends, loopHead, ensures, blocks, gen, translator, prover, scopeVars);
    }

    private static VCExpr getLoopPreCondition(VCExpr requires, Block start, Block loopHead, VCExpressionGenerator gen, Boogie2VCExprTranslator translator, ProverInterface prover, IEnumerable<Variable> scopeVars) {
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

      return setSP(start, loopHead, requires, blocks, gen, translator, prover, scopeVars);
    }
    
    private static VCExpr weakestPrecondition(Cmd cmd, VCExpr Q, VCExpressionGenerator gen, Boogie2VCExprTranslator translator, ProverInterface prover, IEnumerable<Variable> scopeVars) {
      if (cmd is AssertCmd) {
        // assert A -> A && Q
        AssertCmd ac = (AssertCmd) cmd;
        VCExpr A = translator.Translate(ac.Expr);

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
        foreach (IdentifierExpr i in hc.Vars) {
          VCExprVar v = (VCExprVar)translator.Translate(i);
          if (QFree.Contains(v)) {
            vars.Add(v);
          }
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

          return gen.Forall(freshVars, new List<VCTrigger>(), substQ);
          //return prover.EliminateQuantifiers(gen.Forall(freshVars, new List<VCTrigger>(), substQ), scopeVars);
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
          QCall = weakestPrecondition(c, QCall, gen, translator, prover, scopeVars);
        }
        return QCall;
      }
      throw new NotImplementedException("unimplemented command for WP: " + cmd.ToString());
    }

    private static VCExpr strongestPostcondition(Cmd cmd, VCExpr P, VCExpressionGenerator gen, Boogie2VCExprTranslator translator, ProverInterface prover, IEnumerable<Variable> scopeVars) {
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
        foreach (IdentifierExpr i in hc.Vars) {
          vars.Add((VCExprVar)translator.Translate(i));
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
          return P;
        } else {
          VCExprSubstitution subst = new VCExprSubstitution(toSubst, new Dictionary<TypeVariable, Type>());
          SubstitutingVCExprVisitor substituter = new SubstitutingVCExprVisitor(gen);
          VCExpr substP = substituter.Mutate(P, subst);

          //return substP;
          return gen.Exists(freshVars, new List<VCTrigger>(), substP);
          //return prover.EliminateQuantifiers(gen.Exists(freshVars, new List<VCTrigger>(), substP), scopeVars);
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
          //return prover.EliminateQuantifiers(gen.Exists(freshVars, new List<VCTrigger>(), substP), scopeVars);
          return gen.Exists(freshVars, new List<VCTrigger>(), substP);
        }

      } else if (cmd is CallCmd) {
        CallCmd callCmd = (CallCmd)cmd;
        StateCmd desugar = callCmd.Desugaring as StateCmd;
        VCExpr PCall = P;
        foreach (Cmd c in desugar.Cmds) {
          PCall = strongestPostcondition(c, PCall, gen, translator, prover, scopeVars);
        }
        return PCall;
      }
      throw new NotImplementedException("unimplemented command for SP: " + cmd.ToString());
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
  }
  
}

