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
      VCExpressionGenerator gen = new VCExpressionGenerator();
      List<string> proverCommands = new List<string>();
      proverCommands.Add("smtlib");
      proverCommands.Add("z3");
      VCGenerationOptions genOptions = new VCGenerationOptions(proverCommands);
      Boogie2VCExprTranslator translator = new Boogie2VCExprTranslator(gen, genOptions);

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
                  InferLoopInvariant(program, procedureImplementations, impl, b, gen, translator);
                }
              }
            }
          }
        }
      }
    }

    private static void InferLoopInvariant(Program program, Dictionary<Procedure, Implementation[]> procImpl, Implementation impl, Block loopHead,
      VCExpressionGenerator gen, Boogie2VCExprTranslator translator) {
      Block start = impl.Blocks[0];
      Debug.Print("start block: " + start.ToString());
      Block end = getEndBlock(impl);
      Debug.Print("end block: " + end.ToString());

      getPostCondition(end, loopHead);
      getPreCondition(start, loopHead);
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

    private static List<List<Block>> getTracesForward(Block start, Block end) {
      List<List<Block>> paths = new List<List<Block>>();
      paths.Add(new List<Block>());
      DoDFSVisit(start, end, paths);
      return paths;
    }

    private static List<List<Block>> getTracesBackward(Block start, Block end) {


      return null;
    }

    private static void DoDFSVisit(Block block, Block target, List<List<Block>> paths) {

      // case 1. We visit the target => We are done

      if (block == target) {
        paths.Last().Add(target);
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
        foreach (Block nextBlock in successors.labelTargets) {
          // Otherwise we perform the DFS visit
          paths.Last().Add(nextBlock);
          DoDFSVisit(nextBlock, target, paths);

          // need to move to next list for further iteration
        }
      }
    }

    private static void getPostCondition(Block end, Block loopHead) {
      List<List<Block>> traces;
    }

    private static void getPreCondition(Block start, Block loopHead) {
      List<List<Block>> traces;
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
        // x := e -> exists x' :: P[x\x'] && e[x\x']
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

