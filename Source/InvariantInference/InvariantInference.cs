using System;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.Boogie.VCExprAST;

namespace Microsoft.Boogie.InvariantInference {
  public class InvariantInference {
    public static void RunInvariantInference(Program program) {
      // find back edges
      WidenPoints.Compute(program);
      Dictionary<Procedure, Implementation[]> procedureImplementations = ComputeProcImplMap(program);

      ProverInterface prover = ProverInterface.CreateProver(program, CommandLineOptions.Clo.ProverLogFilePath,
        CommandLineOptions.Clo.ProverLogFileAppend, CommandLineOptions.Clo.TimeLimit);
      VCExpressionGenerator gen = proverInterface.Context.ExprGen;
      Boogie2VCExprTranslator translator = proverInterface.Context.BoogieExprTranslator;

      // analyze each procedure
      foreach (var proc in program.Procedures) {
        if (procedureImplementations.ContainsKey(proc)) {
          // analyze each implementation of the procedure
          foreach (var impl in procedureImplementations[proc]) {
            // find back edge & check only one back edge per procedure implementation
            bool backEdgeFound = false;
            foreach (Block b in impl.Blocks) {
              if (b.widenBlock == true) {
                if (backEdgeFound == true) {
                  Debug.Print("error: multiple back edges");
                  // error
                } else {
                  backEdgeFound = true;
                  Debug.Print("printing loop path");
                  List<Block> loopBody = WidenPoints.ComputeLoopBodyFrom(b);
                  foreach (Block l in loopBody) {
                    Debug.Print(l.ToString());
                  }
                  VCExpr invariant = InferLoopInvariant(program, procedureImplementations, impl, b, prover);
                }
              }
            }
          }
        }
      }
    }

    private static VCExpr InferLoopInvariant(Program program, Dictionary<Procedure, Implementation[]> procImpl, Implementation impl, Block loopHead,
      ProverInterface prover) {
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

      List<List<Block>> loopPaths = getLoopPaths(loopHead);

      // backward squeezing algorithm
      VCExpr K = getLoopGuard(loopPaths, gen, translator); // guard 
      List<VCExpr> A = new List<VCExpr> { loopP }; //A_0 = P
      List<VCExpr> B = new List<VCExpr> { gen.AndSimp(gen.NotSimp(K), gen.NotSimp(loopQ)) }; //B_0 = !K && !Q
      List<VCExpr> I = new List<VCExpr>();
      int t = 0;
      int r = 0;

      while (true) {
        VCExpr ADisjunct = listDisjunction(A, gen);
        if (!satisfiable(gen.AndSimp(B[r], ADisjunct), prover)) {
          I[r] = calculateInterpolant(B[r], ADisjunct);
          VCExpr notI_r = gen.NotSimp(I[r]);
          if (isInductive(notI_r, loopPaths, prover)) {
            return notI_r; // found invariant
          }
          B[r + 1] = gen.OrSimp(I[r], pathWP(loopPaths, I[r], gen, translator)); // guard is implicitly included in WP via boogie's assumes
          r++;
          A[t + 1] = pathSP(loopPaths, A[t], gen, translator); // guard is implicitly included in SP via boogie's assumes
          t++; 
        } else {
          if (isConcrete(B[r])) {
            return VCExpressionGenerator.True; // fail to find invariant
          } else {
            while (!isConcrete(B[r])) {
              r--;
              B[r + 1] = gen.OrSimp(B[0], pathWP(loopPaths, B[r], gen, translator)); // guard is implicitly included in WP via boogie's assumes
              r++;
            }
          }
        }
      }
    }
    
    // very naive implementation, will surely need to make more sophisticated, just expects loop guard to be in assume statement at start of second block of any loop path
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

    private static List<List<Block>> getLoopPaths(Block loopHead) {

    }

    private static bool satisfiable(VCExpr predicate, ProverInterface prover) {
      return true;
    }

    // calculates craig interpolant for A w.r.t. B
    private static VCExpr calculateInterpolant(VCExpr A, VCExpr B) {

    }

    private static bool isInductive(VCExpr invarCandidate, List<List<Block>> paths, ProverInterface prover) {
      Boogie2VCExprTranslator translator = prover.Context.BoogieExprTranslator;
      VCExpressionGenerator gen = prover.Context.ExprGen;
      VCExpr loopBodyWP = pathWP(paths, invarCandidate, gen, translator);
      VCExpr vc = gen.NotSimp(gen.ImpliesSimp(invarCandidate, loopBodyWP)); // guard is implicitly included in WP via boogie's assumes
      return !satisfiable(vc, prover);
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
    
    private static void DoDFSVisit(Block block, Block target, List<List<Block>> paths) {
      // case 1. We visit the target => We are done
      if (block == target) {
        paths.Last().Remove(paths.Last().Last()); // don't want target in path
        return;
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

    private static void DoBackwardsDFSVisit(Block block, Block target, List<List<Block>> paths) {
      // case 1. We visit the target => We are done
      if (block == target) {
        paths.Last().Remove(paths.Last().Last()); // don't want target in path
        return; 
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
          if (!firstPredecessor) {
            firstPredecessor = true;
          } else {
            // create new path for new branch
            paths.Add(branchPoint.ToList());
          }
          paths.Last().Add(nextBlock);
          DoDFSVisit(nextBlock, target, paths);
        }
      }
    }

    private static VCExpr pathSP(List<List<Block>> paths, VCExpr P, VCExpressionGenerator gen, Boogie2VCExprTranslator translator) {
      if (paths.Count == 0) {
        Debug.Print("error: no paths to calculate SP on");
      }
      VCExpr sp = VCExpressionGenerator.False; // default to be replaced in later or
      foreach (List<Block> path in paths) {
        foreach (Block block in path) {
          foreach(Cmd cmd in block.Cmds) {
            P = strongestPostcondition(cmd, P, gen, translator);
          }
        }
        sp = gen.OrSimp(sp, P);
      }
      return sp;
    }

    private static VCExpr pathWP(List<List<Block>> paths, VCExpr Q, VCExpressionGenerator gen, Boogie2VCExprTranslator translator) {
      if (paths.Count == 0) {
        Debug.Print("error: no paths to calculate WP on");
      }
      VCExpr wp = VCExpressionGenerator.False; // default to be replaced in later or
      foreach (List<Block> path in paths) {
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

        foreach (VCExprVar v in vars) {
          VCExpr fresh = gen.Variable(v.Name + "'", v.Type);
          toSubst.Add(v, fresh);
        }

        VCExprSubstitution subst = new VCExprSubstitution(toSubst, new Dictionary<TypeVariable, Type>());
        SubstitutingVCExprVisitor substituter = new SubstitutingVCExprVisitor(gen);
        VCExpr substP = substituter.Mutate(P, subst);

        return gen.Exists(vars, new List<VCTrigger>(), substP);

      } else if (cmd is AssignCmd) {
        // x := e -> exists x' :: P[x\x'] && x == e[x\x']
        AssignCmd ac = (AssignCmd)cmd;
        ac = ac.AsSimpleAssignCmd;

        List<(VCExpr, VCExpr)> assignments = new List<(VCExpr, VCExpr)>();
        Dictionary<VCExprVar, VCExpr> toSubst = new Dictionary<VCExprVar, VCExpr>();
        for (int i = 0; i < ac.Lhss.Count; ++i) {
          IdentifierExpr lhs = ((SimpleAssignLhs)ac.Lhss[i]).AssignedVariable;
          Expr rhs = ac.Rhss[i];
          VCExprVar lhsPred = (VCExprVar)translator.Translate(lhs);
          VCExpr rhsPred = translator.Translate(rhs);
          assignments.Add((lhsPred, rhsPred));

          VCExpr fresh = gen.Variable(lhsPred.Name + "'", lhsPred.Type);
          toSubst.Add(lhsPred, fresh);
        }

        VCExprSubstitution subst = new VCExprSubstitution(toSubst, new Dictionary<TypeVariable, Type>());
        SubstitutingVCExprVisitor substituter = new SubstitutingVCExprVisitor(gen);
        VCExpr sp = substituter.Mutate(P, subst);

        foreach ((VCExpr, VCExpr) assign in assignments) {
          VCExpr substRhs = substituter.Mutate(assign.Item2, subst);
          sp = gen.AndSimp(sp, gen.Eq(assign.Item1, substRhs));
        }

        return sp;
      } else {
        Debug.Print("error: unimplemented command for WP: " + cmd.ToString());
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

