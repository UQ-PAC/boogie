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

      MathSAT mathSAT = MathSAT.CreateProver(program, CommandLineOptions.Clo.MathSATLogFilePath,
        CommandLineOptions.Clo.MathSATLogFileAppend, CommandLineOptions.Clo.TimeLimit);

      // z3 apparently has a new interpolant algorithm - we should try it as well to compare

      // analyze each procedure
      foreach (var proc in program.Procedures) {
        if (procedureImplementations.ContainsKey(proc)) {
          // analyze each implementation of the procedure
          foreach (var impl in procedureImplementations[proc]) {
            // find back edge & check only one back edge per procedure implementation
            bool backEdgeFound = false;
            foreach (Block b in impl.Blocks) {
              if (b.widenBlock) {
                if (backEdgeFound) {
                  Debug.Print("error: multiple back edges");
                  // error
                } else {
                  backEdgeFound = true;
                  Debug.Print("printing loop path");
                  List<Block> loopBody = WidenPoints.ComputeLoopBodyFrom(b);
                  foreach (Block l in loopBody) {
                    Debug.Print(l.ToString());
                  }
                  // naive attempt at getting variables that may be referred to
                  IEnumerable<Variable> scopeVars = new List<Variable>();
                  scopeVars = scopeVars.Concat(program.GlobalVariables);
                  scopeVars = scopeVars.Concat(impl.LocVars);

                  VCExpr invariant = InferLoopInvariant(program, procedureImplementations, impl, b, prover, mathSAT, scopeVars);
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

    private static Expr VCtoExpr(VCExpr vc, IEnumerable<Variable> scopeVars) {
      if (vc == VCExpressionGenerator.True) {
        return new LiteralExpr(Token.NoToken, true);
      } else if (vc == VCExpressionGenerator.False) {
        return new LiteralExpr(Token.NoToken, false);
      } else if (vc is VCExprIntLit) {
        return new LiteralExpr(Token.NoToken, ((VCExprIntLit)vc).Val);
      } else if (vc is VCExprNAry) {
        VCExprNAry nary = vc as VCExprNAry;
        VCExprOp op = nary.Op;
        if (op == VCExpressionGenerator.AddIOp) {
          return Expr.Add(VCtoExpr(nary[0], scopeVars), VCtoExpr(nary[1], scopeVars));
        } else if (op == VCExpressionGenerator.SubIOp) {
          return Expr.Sub(VCtoExpr(nary[0], scopeVars), VCtoExpr(nary[1], scopeVars));
        } else if (op == VCExpressionGenerator.MulIOp) {
          return Expr.Mul(VCtoExpr(nary[0], scopeVars), VCtoExpr(nary[1], scopeVars));
        } else if (op == VCExpressionGenerator.DivIOp) {
          return Expr.Div(VCtoExpr(nary[0], scopeVars), VCtoExpr(nary[1], scopeVars));
        } else if (op == VCExpressionGenerator.ModOp) {
          return Expr.Mod(VCtoExpr(nary[0], scopeVars), VCtoExpr(nary[1], scopeVars));
        } else if (op == VCExpressionGenerator.EqOp) {
          return Expr.Eq(VCtoExpr(nary[0], scopeVars), VCtoExpr(nary[1], scopeVars));
        } else if (op == VCExpressionGenerator.GtOp) {
          return Expr.Gt(VCtoExpr(nary[0], scopeVars), VCtoExpr(nary[1], scopeVars));
        } else if (op == VCExpressionGenerator.LtOp) {
          return Expr.Lt(VCtoExpr(nary[0], scopeVars), VCtoExpr(nary[1], scopeVars));
        } else if (op == VCExpressionGenerator.LeOp) {
          return Expr.Le(VCtoExpr(nary[0], scopeVars), VCtoExpr(nary[1], scopeVars));
        } else if (op == VCExpressionGenerator.GeOp) {
          return Expr.Ge(VCtoExpr(nary[0], scopeVars), VCtoExpr(nary[1], scopeVars));
        } else if (op == VCExpressionGenerator.NotOp) {
          return Expr.Not(VCtoExpr(nary[0], scopeVars));
        } else if (op == VCExpressionGenerator.AndOp) {
          return Expr.And(VCtoExpr(nary[0], scopeVars), VCtoExpr(nary[1], scopeVars));
        } else if (op == VCExpressionGenerator.OrOp) {
          return Expr.Or(VCtoExpr(nary[0], scopeVars), VCtoExpr(nary[1], scopeVars));
        } else if (op == VCExpressionGenerator.ImpliesOp) {
          return Expr.Imp(VCtoExpr(nary[0], scopeVars), VCtoExpr(nary[1], scopeVars));
        }
      } else if (vc is VCExprVar) {
        VCExprVar vcVar = vc as VCExprVar;
        foreach (Variable v in scopeVars) {
          // this is a little naive - variables can be distinct yet share a name - but shouldn't be an issue here
          if (vcVar.Name == v.Name) {
            IdentifierExpr i = new IdentifierExpr(Token.NoToken, vcVar.Name);
            i.Decl = v;
            return i;
          }
        }
      }

      return null;
    }

    private static VCExpr InferLoopInvariant(Program program, Dictionary<Procedure, Implementation[]> procImpl, Implementation impl, Block loopHead,
      ProverInterface prover, MathSAT mathSAT, IEnumerable<Variable> scopeVars) {
      Boogie2VCExprTranslator translator = prover.Context.BoogieExprTranslator;
      VCExpressionGenerator gen = prover.Context.ExprGen;

      Block start = impl.Blocks[0];
      Debug.Print("start block: " + start.ToString());
      Block end = getEndBlock(impl);
      Debug.Print("end block: " + end.ToString());

      VCExpr requires = convertRequires(impl, gen, translator);
      VCExpr ensures = convertEnsures(impl, gen, translator);

      VCExpr loopP = getLoopPreCondition(requires, start, loopHead, gen, translator);
      VCExpr loopQ = getLoopPostCondition(ensures, end, loopHead, gen, translator);

      List<List<Block>> loopPaths = new List<List<Block>>();
      DoDFSVisitLoop(loopHead, loopHead, loopPaths);
      List<List<Block>> loopPathsBack = new List<List<Block>>();
      DoBackwardsDFSVisitLoop(loopHead, loopHead, loopPathsBack);

      // backward squeezing algorithm
      VCExpr K = getLoopGuard(loopPaths, gen, translator); // guard 
      List<VCExpr> A = new List<VCExpr> { loopP }; //A_0 = P
      List<VCExpr> B = new List<VCExpr> { gen.AndSimp(gen.NotSimp(K), gen.NotSimp(loopQ)) }; //B_0 = !K && !Q
      int t = 0;
      int r = 0;
      int concrete = 0;

      while (true) {
        VCExpr ADisjunct = listDisjunction(A, gen);
        if (!satisfiable(gen.AndSimp(B[r], ADisjunct), prover)) { // just for testing
          VCExpr I = CalculateInterpolant(B[r], ADisjunct, prover, mathSAT, scopeVars);
          VCExpr notI_r = gen.NotSimp(I);
          if (isInductive(notI_r, loopPathsBack, prover)) {
            return notI_r; // found invariant
          }
          B.Insert(r + 1, gen.OrSimp(I, pathWP(loopPathsBack, I, gen, translator))); // guard is implicitly included in WP via boogie's assumes
          r++;
          A.Insert(t + 1, pathSP(loopPaths, A[t], gen, translator)); // guard is implicitly included in SP via boogie's assumes
          t++; 
        } else {
          if (r <= concrete) {
            return VCExpressionGenerator.True; // fail to find invariant
          } else {
            r = concrete;
            B.Insert(r + 1, gen.OrSimp(B[0], pathWP(loopPathsBack, B[r], gen, translator))); // guard is implicitly included in WP via boogie's assumes
            r++;
          }
        }
      }
    }

    private static VCExpr CalculateInterpolant(VCExpr A, VCExpr B, ProverInterface prover, MathSAT mathSAT, IEnumerable<Variable> scopeVars) {
      // need to only apply quantifier elimination if vcexpr contains quantifiers
      // also need to remove ticklebool if possible - means qe result is unnecessarily complex
      string AElim = prover.EliminateQuantifiers(A);
      string BElim = prover.EliminateQuantifiers(B);

      SExpr output = mathSAT.calculateInterpolant(A, B, AElim, BElim);
      return StoVC(output, prover.Context.ExprGen, prover.Context.BoogieExprTranslator, scopeVars);
    }

    private static VCExpr StoVC(SExpr sexpr, VCExpressionGenerator gen, Boogie2VCExprTranslator translator, IEnumerable<Variable> scopeVars) {
      // assuming everything is ints for now, figure out rest later
      List<VCExpr> args = new List<VCExpr>();
      switch (sexpr.Name) {
        case "and":
          return gen.AndSimp(StoVC(sexpr.Arguments[0], gen, translator, scopeVars), StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
        case "or":
          return gen.OrSimp(StoVC(sexpr.Arguments[0], gen, translator, scopeVars), StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
        case "not":
          return gen.NotSimp(StoVC(sexpr.Arguments[0], gen, translator, scopeVars));
        case "=>":
          return gen.ImpliesSimp(StoVC(sexpr.Arguments[0], gen, translator, scopeVars), StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
        case "+":
          args.Add(StoVC(sexpr.Arguments[0], gen, translator, scopeVars));
          args.Add(StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
          return gen.Function(VCExpressionGenerator.AddIOp, args);
        case "-":
          if (sexpr.ArgCount == 1) {
            args.Add(gen.Integer(BigNum.ZERO));
            args.Add(StoVC(sexpr.Arguments[0], gen, translator, scopeVars));
          } else if (sexpr.ArgCount == 2) {
            args.Add(StoVC(sexpr.Arguments[0], gen, translator, scopeVars));
            args.Add(StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
          }
          return gen.Function(VCExpressionGenerator.SubIOp, args);
        case "*":
          args.Add(StoVC(sexpr.Arguments[0], gen, translator, scopeVars));
          args.Add(StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
          return gen.Function(VCExpressionGenerator.MulIOp, args);
        case "div":
          args.Add(StoVC(sexpr.Arguments[0], gen, translator, scopeVars));
          args.Add(StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
          return gen.Function(VCExpressionGenerator.DivIOp, args);
        case "mod":
          args.Add(StoVC(sexpr.Arguments[0], gen, translator, scopeVars));
          args.Add(StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
          return gen.Function(VCExpressionGenerator.ModOp, args);
        case "=":
          args.Add(StoVC(sexpr.Arguments[0], gen, translator, scopeVars));
          args.Add(StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
          return gen.Function(VCExpressionGenerator.EqOp, args);
        case ">":
          args.Add(StoVC(sexpr.Arguments[0], gen, translator, scopeVars));
          args.Add(StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
          return gen.Function(VCExpressionGenerator.GtOp, args);
        case "<":
          args.Add(StoVC(sexpr.Arguments[0], gen, translator, scopeVars));
          args.Add(StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
          return gen.Function(VCExpressionGenerator.LtOp, args);
        case "<=":
          args.Add(StoVC(sexpr.Arguments[0], gen, translator, scopeVars));
          args.Add(StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
          return gen.Function(VCExpressionGenerator.LeOp, args);
        case ">=":
          args.Add(StoVC(sexpr.Arguments[0], gen, translator, scopeVars));
          args.Add(StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
          return gen.Function(VCExpressionGenerator.GeOp, args);
        case "ite":
          args.Add(StoVC(sexpr.Arguments[0], gen, translator, scopeVars));
          args.Add(StoVC(sexpr.Arguments[1], gen, translator, scopeVars));
          args.Add(StoVC(sexpr.Arguments[2], gen, translator, scopeVars));
          return gen.Function(VCExpressionGenerator.IfThenElseOp, args);
        case "true":
          return VCExpressionGenerator.True;
        case "false":
          return VCExpressionGenerator.False;
        case "abs":
        case "distinct":
          // need to figure out these?
          return null;
        default:
          if (sexpr.Name.All(char.IsDigit)) {
            // int
            return gen.Integer(BigNum.FromString(sexpr.Name));
          } else {
            // identifier
            foreach (Variable v in scopeVars) {
              if (v.Name == sexpr.Name) {
                return translator.LookupVariable(v);
              }
            }
          }
          // can figure out floats, reals later
          break;
      }
      return null;
    }

    // very naive implementation, will surely need to make more sophisticated, just expects loop guard to be in assume statement at start of second block of any loop path
    // works in basic cases at least
    private static VCExpr getLoopGuard(List<List<Block>> paths, VCExpressionGenerator gen, Boogie2VCExprTranslator translator) {
      Block loopHead = paths[0][0];
      GotoCmd successors = (GotoCmd)loopHead.TransferCmd;
      if (successors.labelTargets != null) {
        foreach (Block nextBlock in successors.labelTargets) {
          foreach (List<Block> path in paths) {
            if (path[1] == nextBlock) {
              if (nextBlock.Cmds[0] is AssumeCmd) {
                AssumeCmd ac = (AssumeCmd)nextBlock.Cmds[0];
                return translator.Translate(ac.Expr);
              }
            }
          }
        }
      }
      return VCExpressionGenerator.True; // failure
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
          Debug.Print("error: " + outcome);
          break;
      }
      return false;
    }

    private static bool isInductive(VCExpr invarCandidate, List<List<Block>> paths, ProverInterface prover) {
      Boogie2VCExprTranslator translator = prover.Context.BoogieExprTranslator;
      VCExpressionGenerator gen = prover.Context.ExprGen;
      VCExpr loopBodyWP = pathWP(paths, invarCandidate, gen, translator);
      VCExpr imp = gen.ImpliesSimp(invarCandidate, loopBodyWP); // guard is implicitly included in WP via boogie's assumes
      return satisfiable(imp, prover);
    }

    private static bool isConcrete(VCExpr pred) {
      return true;
    }

    private static VCExpr listDisjunction(List<VCExpr> list, VCExpressionGenerator gen) {
      VCExpr disjunction = list[0];
      for (int i = 1; i < list.Count; i++) {
        gen.OrSimp(disjunction, list[i]);
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

    private static Block getEndBlock(Implementation impl) {
      Block end = null;
      foreach (Block b in impl.Blocks) {
        if (b.TransferCmd is ReturnCmd) {
          if (end != null) {
            Debug.Print("error: multiple exits");
          }
          end = b;
        }
      }
      return end;
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

    private static void DoBackwardsDFSVisitLoop(Block block, Block loopHead, List<List<Block>> paths) {
      // case 1. We visit the loop head
      if (block == loopHead && !paths.Any()) {
        // initial case
        paths.Add(new List<Block> { loopHead });
      }

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
          if (nextBlock != loopHead) {
            if (!firstPredecessor) {
              firstPredecessor = true;
            } else {
              // create new path for new branch
              paths.Add(branchPoint.ToList());
            }
            paths.Last().Add(nextBlock);
            DoBackwardsDFSVisitLoop(nextBlock, loopHead, paths);
          }
        }
      }
    }

    private static VCExpr pathSP(List<List<Block>> paths, VCExpr P_In, VCExpressionGenerator gen, Boogie2VCExprTranslator translator) {
      if (paths.Count == 0) {
        Debug.Print("error: no paths to calculate SP on");
      }
      VCExpr sp = VCExpressionGenerator.False; // default to be replaced in later or
      foreach (List<Block> path in paths) {
        VCExpr P = P_In;
        foreach (Block block in path) {
          foreach(Cmd cmd in block.Cmds) {
            P = strongestPostcondition(cmd, P, gen, translator);
          }
        }
        sp = gen.OrSimp(sp, P);
      }
      return sp;
    }

    private static VCExpr pathWP(List<List<Block>> paths, VCExpr Q_In, VCExpressionGenerator gen, Boogie2VCExprTranslator translator) {
      if (paths.Count == 0) {
        Debug.Print("error: no paths to calculate WP on");
      }
      VCExpr wp = VCExpressionGenerator.False; // default to be replaced in later or
      foreach (List<Block> path in paths) {
        VCExpr Q = Q_In;
        foreach (Block block in path) {
          for (int i = block.Cmds.Count; --i >= 0;) {
            Q = weakestPrecondition(block.Cmds[i], Q, gen, translator);
          }
        }
        wp = gen.OrSimp(wp, Q);
      }
      return wp;
    }

    private static VCExpr getLoopPostCondition(VCExpr ensures, Block end, Block loopHead, VCExpressionGenerator gen, Boogie2VCExprTranslator translator) {
      List<List<Block>> paths = new List<List<Block>>();
      paths.Add(new List<Block> { end });
      DoBackwardsDFSVisit(end, loopHead, paths);

      return pathWP(paths, ensures, gen, translator);
    }

    private static VCExpr getLoopPreCondition(VCExpr requires, Block start, Block loopHead, VCExpressionGenerator gen, Boogie2VCExprTranslator translator) {
      List<List<Block>> paths = new List<List<Block>>();
      paths.Add(new List<Block> { start });
      DoDFSVisit(start, loopHead, paths);

      return pathSP(paths, requires, gen, translator);
    }
    
    private static VCExpr weakestPrecondition(Cmd cmd, VCExpr Q, VCExpressionGenerator gen, Boogie2VCExprTranslator translator) {
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
        foreach (IdentifierExpr i in hc.Vars) {
          vars.Add((VCExprVar) translator.Translate(i));
        }

        /*
        // need to substitute all havoced vars for fresh vars to be bound???

        VCExprSubstitution subst = new VCExprSubstitution( , new Dictionary<TypeVariable, Type>());
        SubstitutingVCExprVisitor substituter = new SubstitutingVCExprVisitor(gen);
        VCExpr substQ = substituter.Mutate(Q, subst);
        */

        return gen.Forall(vars, new List<VCTrigger>(), Q);

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

        VCExprSubstitution subst = new VCExprSubstitution(assignments , new Dictionary<TypeVariable, Type>());
        SubstitutingVCExprVisitor substituter = new SubstitutingVCExprVisitor(gen);
        VCExpr substQ = substituter.Mutate(Q, subst);

        return substQ; 
      } else {
        Debug.Print("error: unimplemented command for WP: " + cmd.ToString());
      }
      return null;
    }

    private static VCExpr strongestPostcondition(Cmd cmd, VCExpr P, VCExpressionGenerator gen, Boogie2VCExprTranslator translator) {
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

        foreach (VCExprVar v in vars) {
          VCExprVar fresh = gen.Variable(v.Name + "'", v.Type); // need to make more sophisticated
          toSubst.Add(v, fresh);
          freshVars.Add(fresh);
        }

        VCExprSubstitution subst = new VCExprSubstitution(toSubst, new Dictionary<TypeVariable, Type>());
        SubstitutingVCExprVisitor substituter = new SubstitutingVCExprVisitor(gen);
        VCExpr substP = substituter.Mutate(P, subst);

        return gen.Exists(freshVars, new List<VCTrigger>(), substP);

      } else if (cmd is AssignCmd) {
        // x := e -> exists x' :: P[x\x'] && x == e[x\x']
        AssignCmd ac = (AssignCmd)cmd;
        ac = ac.AsSimpleAssignCmd;

        List<(VCExpr, VCExpr)> assignments = new List<(VCExpr, VCExpr)>();
        Dictionary<VCExprVar, VCExpr> toSubst = new Dictionary<VCExprVar, VCExpr>();
        List<VCExprVar> freshVars = new List<VCExprVar>();
        for (int i = 0; i < ac.Lhss.Count; ++i) {
          IdentifierExpr lhs = ((SimpleAssignLhs)ac.Lhss[i]).AssignedVariable;
          Expr rhs = ac.Rhss[i];
          VCExprVar lhsPred = (VCExprVar)translator.Translate(lhs);
          VCExpr rhsPred = translator.Translate(rhs);
          assignments.Add((lhsPred, rhsPred));

          VCExprVar fresh = gen.Variable(lhsPred.Name + "'", lhsPred.Type); // need to make more sophisticated
          toSubst.Add(lhsPred, fresh);
          freshVars.Add(fresh);
        }

        bool needExists = false; // need to optimise out case where x not in RHS or P but this doesn't really work at present, need to fix somehow
        VCExprSubstitution subst = new VCExprSubstitution(toSubst, new Dictionary<TypeVariable, Type>());
        SubstitutingVCExprVisitor substituter = new SubstitutingVCExprVisitor(gen);
        VCExpr substP = substituter.Mutate(P, subst);
        if (substP != P) {
          needExists = true;
        }

        foreach ((VCExpr, VCExpr) assign in assignments) {
          VCExpr substRhs = substituter.Mutate(assign.Item2, subst);
          if (substRhs != assign.Item2) {
            needExists = true;
          }
          substP = gen.AndSimp(substP, gen.Eq(assign.Item1, substRhs));
        }
        VCExpr sp = substP;
        if (needExists) {
          sp = gen.Exists(freshVars, new List<VCTrigger>(), substP);
        }

        return sp;
      } else {
        Debug.Print("error: unimplemented command for SP: " + cmd.ToString());
      }
      return null;
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

