using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Boogie.VCExprAST;
using Microsoft.Boogie.SMTLib;
using Microsoft.BaseTypes;
using Microsoft.Boogie.GraphUtil;
using VC;



namespace Microsoft.Boogie.InvariantInference {
  class Passifier {

    private Implementation impl;
    private HashSet<Block> loopBody;
    private HashSet<Block> beforeLoop;
    private HashSet<Block> afterLoop;
    private Block loopHead;
    private Program program;
    private Dictionary<Variable, int> varToIncarnationForward;
    private Dictionary<Variable, int> varToIncarnationBackward;
    private Dictionary<Variable, Expr> loopStartIncarnationsForward;
    private Dictionary<Variable, Expr> loopEndIncarnationsForward;
    private Dictionary<Variable, Expr> loopStartIncarnationsBackward;
    private Dictionary<Variable, Expr> loopEndIncarnationsBackward;
    private Dictionary<Block, Block> origToForward;
    private Dictionary<Block, Block> origToBackward;
    private Implementation forwardPassive;
    private Implementation backwardPassive;
    bool loopBodyOnly = false;

    public Passifier(Program program, Implementation impl, HashSet<Block> loopBody, HashSet<Block> beforeLoop, HashSet<Block> afterLoop, Block loopHead) {
      this.program = program;
      this.impl = impl;
      this.loopBody = loopBody;
      this.beforeLoop = beforeLoop;
      this.afterLoop = afterLoop;
      this.loopHead = loopHead;

      varToIncarnationBackward = new Dictionary<Variable, int>();
      varToIncarnationForward = new Dictionary<Variable, int>();

      // initialise - insert preconditions, postconditions, unified exit, join blocks?
      // need unified back edge block too ?
      InsertPreCondition(impl.Blocks[0]);
      InsertPostCondition(impl.Blocks);

      if (CommandLineOptions.Clo.TraceVerify) {
        Console.WriteLine("before passifying");
        ConditionGeneration.EmitImpl(impl, false);
      }

      // duplicate impl into two versions, forward and back
      Duplicator duplicator = new Duplicator();
      Implementation forwardPassive = duplicator.VisitImplementation(impl);
      Implementation backwardPassive = duplicator.VisitImplementation(impl);

      // need mapping from original impl to new impls
      origToForward = new Dictionary<Block, Block>();
      foreach (var blockPair in impl.Blocks.Zip(forwardPassive.Blocks)) {
        origToForward.Add(blockPair.Item1, blockPair.Item2);
      }

      origToBackward = new Dictionary<Block, Block>();
      foreach (var blockPair in impl.Blocks.Zip(backwardPassive.Blocks)) {
        origToBackward.Add(blockPair.Item1, blockPair.Item2);
      }

      // remove all after loop blocks from forward, all before loop blocks from back

      HashSet<Block> dupeBeforeLoopForward = new HashSet<Block>();
      foreach (Block b in beforeLoop) {
        backwardPassive.Blocks.Remove(origToBackward[b]);
        dupeBeforeLoopForward.Add(origToForward[b]);
      }

      HashSet<Block> dupeAfterLoopForward = new HashSet<Block>();
      foreach (Block b in afterLoop) {
        Block clone = origToForward[b];
        forwardPassive.Blocks.Remove(clone);
        dupeAfterLoopForward.Add(clone);
      }

      Block forwardEnd = new Block(Token.NoToken, "ForwardLoopEnd", new List<Cmd>(), new ReturnCmd(Token.NoToken));
      forwardPassive.Blocks.Add(forwardEnd);

      Block backwardEnd = new Block(Token.NoToken, "BackwardLoopEnd", new List<Cmd>(), new ReturnCmd(Token.NoToken));
      backwardPassive.Blocks.Add(backwardEnd);

      foreach (Block b in forwardPassive.Blocks) {
        if (b.TransferCmd is GotoCmd gt) {
          for (int i = 0; i < gt.labelTargets.Count; i++) {
            Block successor = gt.labelTargets[i];
            if (dupeAfterLoopForward.Contains(successor)) {
              gt.labelTargets.Remove(successor);
              gt.labelNames.Remove(successor.Label);
            } else if (origToForward[loopHead] == successor && !dupeBeforeLoopForward.Contains(b)) {
              gt.labelTargets.Remove(successor);
              gt.labelNames.Remove(successor.Label);
              gt.AddTarget(forwardEnd);
            }
          }
        }
      }

      foreach (Block b in backwardPassive.Blocks) {
        if (b.TransferCmd is GotoCmd gt) {
          for (int i = 0; i < gt.labelTargets.Count; i++) {
            Block successor = gt.labelTargets[i];
            if (successor == origToBackward[loopHead]) {
              gt.labelTargets.Remove(successor);
              gt.labelNames.Remove(successor.Label);
              gt.AddTarget(backwardEnd);
            }
          }
        }
      }


      AddBlocksBetween(forwardPassive.Blocks);
      AddBlocksBetween(backwardPassive.Blocks);
      
      /*
      Block backwardStart = new Block(Token.NoToken, "BackwardLoopStart", new List<Cmd>(), new ReturnCmd(Token.NoToken));
      backwardPassive.Blocks.Insert(0, backwardStart);

      foreach (Block b in backwardPassive.Blocks) {
        if (b.TransferCmd is GotoCmd gt) {
          for (int i = 0; i < gt.labelTargets.Count; i++) {
            Block successor = gt.labelTargets[i];
            if (origToBackward[loopHead] == successor) {
              gt.labelTargets.Remove(successor);
              gt.labelNames.Remove(successor.Label);
              gt.AddTarget(backwardStart);
            }
          }
        }
      }
      */ 
      forwardPassive.ComputePredecessorsForBlocks();
      backwardPassive.ComputePredecessorsForBlocks();

      loopEndIncarnationsForward = passify(forwardPassive, forwardPassive.Blocks, forwardPassive.Proc.Modifies, true);

      if (CommandLineOptions.Clo.TraceVerify) {
        Console.WriteLine("after passifying forward");
        ConditionGeneration.EmitImpl(forwardPassive, false);
      }

      loopEndIncarnationsBackward = passify(backwardPassive, backwardPassive.Blocks, backwardPassive.Proc.Modifies, false);

      if (CommandLineOptions.Clo.TraceVerify) {
        Console.WriteLine("after passifying backward");
        ConditionGeneration.EmitImpl(backwardPassive, false);
      }



    }


    // do single passification pass on each, do first loop iterations

    private void LoopBodyOnly() {
      foreach (Block b in beforeLoop) {
        forwardPassive.Blocks.Remove(origToForward[b]);
      }
      HashSet<Block> toRemove = new HashSet<Block>();
      foreach (Block b in afterLoop) {
        backwardPassive.Blocks.Remove(origToBackward[b]);
        toRemove.Add(origToBackward[b]);
      }
      foreach (Block b in backwardPassive.Blocks) {
        if (b.TransferCmd is GotoCmd g) {
          foreach (Block t in toRemove) {
            g.labelTargets.Remove(t);
            g.labelNames.Remove(t.Label);
          }
        }
      }
      backwardPassive.ComputePredecessorsForBlocks();
      forwardPassive.ComputePredecessorsForBlocks();
      loopBodyOnly = true;
    }

    public void NextIteration() {
      if (!loopBodyOnly) {
        LoopBodyOnly();
      }


  }

    // then have method to duplicate loop body, update incarnations, and insert at end of loop to unroll

    private Block CreateBlockBetween(Block p, Block s) {
      string newBlockLabel = p.Label + "_@2_" + s.Label;
      Block newBlock = new Block(Token.NoToken, newBlockLabel, new List<Cmd>(), new GotoCmd(Token.NoToken, new List<Block>{ s }));
      GotoCmd g = p.TransferCmd as GotoCmd;
      g.labelTargets.Remove(s);
      g.labelNames.Remove(s.Label);
      g.AddTarget(newBlock);

      return newBlock;
    }

    private void AddBlocksBetween(List<Block> blocks) {
      List<Block> tweens = new List<Block>();
      foreach (Block b in blocks) {
        if (b.Predecessors.Count > 1) {
          foreach (Block p in b.Predecessors) {
            if (p.Exits().Count() > 1) {
              tweens.Add(CreateBlockBetween(p, b));
            }
          }
        }
      }

      blocks.AddRange(tweens);
    }

    private Dictionary<Variable, Expr> ComputeIncarnationMap(Block b, Dictionary<Block, Dictionary<Variable, Expr>> block2Incarnation, bool Forward, Dictionary<Variable, Expr> initialMap) {
      if (b.Predecessors.Count == 0) {
        return new Dictionary<Variable, Expr>(initialMap);
      }
      Dictionary<Variable, Expr> incarnationMap = null;
      HashSet<Variable> fixUpsSet = new HashSet<Variable>();
      List<Variable> fixUps = new List<Variable>();
      foreach (Block pred in b.Predecessors) {
        Dictionary<Variable, Expr> predMap = block2Incarnation[pred];
        if (incarnationMap == null) {
          incarnationMap = new Dictionary<Variable, Expr>(predMap);
          continue;
        }

        List<Variable> conflicts = new List<Variable>();
        foreach (Variable v in incarnationMap.Keys) {
          if (!predMap.ContainsKey(v)) {
            // conflict!!
            conflicts.Add(v);
            if (!fixUpsSet.Contains(v)) {
              fixUps.Add(v);
              fixUpsSet.Add(v);
            }
          }
        }

        // Now that we're done with enumeration, we'll do all the removes
        foreach (Variable v in conflicts) {
          incarnationMap.Remove(v);
        }

        foreach (Variable v in predMap.Keys) {
          if (!incarnationMap.ContainsKey(v)) {
            // v was not in the domain of the predecessors seen so far, so it needs to be fixed up
            if (!fixUpsSet.Contains(v)) {
              fixUps.Add(v);
              fixUpsSet.Add(v);
            }
          } else {
            // v in incarnationMap ==> all pred blocks (up to now) all agree on its incarnation
            if (predMap[v] != incarnationMap[v]) {
              incarnationMap.Remove(v);
              // conflict!!
              if (!fixUpsSet.Contains(v)) {
                fixUpsSet.Add(v);
                fixUps.Add(v);
              }
            }
          }
        }
      }

      foreach (Variable v in fixUps) {
        Variable v_prime = CreateIncarnation(v, Forward);
        IdentifierExpr ie = new IdentifierExpr(v_prime.tok, v_prime);
        incarnationMap[v] = ie;
        foreach (Block pred in b.Predecessors) {
          Dictionary<Variable, Expr> predMap = block2Incarnation[pred];

          Expr pred_incarnation_exp;
          if (predMap.ContainsKey(v)) {
            pred_incarnation_exp = predMap[v];
          } else {
            pred_incarnation_exp = new IdentifierExpr(v.tok, v);
          }

          IdentifierExpr v_prime_exp = new IdentifierExpr(v_prime.tok, v_prime);

          AssumeCmd ac = new AssumeCmd(v.tok, Expr.Eq(v_prime_exp, pred_incarnation_exp));
          pred.Cmds.Add(ac);
        }
      }

      return incarnationMap;

    }
    /*
    private Dictionary<Variable, Expr> ComputeIncarnationMapBackward(Block b, Dictionary<Block, Dictionary<Variable, Expr>> block2Incarnation) {
      if (b.Exits().Count() == 0) {
        return new Dictionary<Variable, Expr>();
      }
      Dictionary<Variable, Expr> incarnationMap = null;
      HashSet<Variable> fixUpsSet = new HashSet<Variable>();
      List<Variable> fixUps = new List<Variable>();
      foreach (Block successor in b.Exits()) {
        Dictionary<Variable, Expr> successorMap = block2Incarnation[successor];
        if (incarnationMap == null) {
          incarnationMap = new Dictionary<Variable, Expr>(successorMap);
          continue;
        }

        List<Variable> conflicts = new List<Variable>();
        foreach (Variable v in incarnationMap.Keys) {
          if (!successorMap.ContainsKey(v)) {
            // conflict!!
            conflicts.Add(v);
            if (!fixUpsSet.Contains(v)) {
              fixUps.Add(v);
              fixUpsSet.Add(v);
            }
          }
        }

        // Now that we're done with enumeration, we'll do all the removes
        foreach (Variable v in conflicts) {
          incarnationMap.Remove(v);
        }

        foreach (Variable v in successorMap.Keys) {
          if (!incarnationMap.ContainsKey(v)) {
            // v was not in the domain of the successors seen so far, so it needs to be fixed up
            fixUps.Add(v);
            fixUpsSet.Add(v);
          } else {
            // v in incarnationMap ==> all successor blocks (up to now) all agree on its incarnation
            if (successorMap[v] != incarnationMap[v]) {
              // conflict!!
              if (!fixUpsSet.Contains(v)) {
                incarnationMap.Remove(v);
                fixUpsSet.Add(v);
                fixUps.Add(v);
              }
            }
          }
        }
      }

      foreach (Variable v in fixUps) {
        Variable v_prime = CreateIncarnation(v, false);
        IdentifierExpr ie = new IdentifierExpr(v_prime.tok, v_prime);
        incarnationMap[v] = ie;
        foreach (Block successor in b.Exits()) {
          Dictionary<Variable, Expr> successorMap = block2Incarnation[successor];

          Expr successor_incarnation_exp;
          if (successorMap.ContainsKey(v)) {
            successor_incarnation_exp = successorMap[v];
          } else {
            successor_incarnation_exp = new IdentifierExpr(v.tok, v);
          }

          IdentifierExpr v_prime_exp = new IdentifierExpr(v_prime.tok, v_prime);

          AssumeCmd ac = new AssumeCmd(v.tok, Expr.Eq(successor_incarnation_exp, v_prime_exp));
          successor.Cmds.Insert(0, ac);
        }
      }

      return incarnationMap;

    }
    */

    private Dictionary<Variable, Expr> passify(Implementation currentImpl, List<Block> blocks, List<IdentifierExpr> modifies, bool Forward) {

      Graph<Block> g = Program.GraphFromBlocks(blocks);
      IEnumerable<Block> sorted = g.TopologicalSort();

      Dictionary<Variable, Expr> oldFrameMap = new Dictionary<Variable, Expr>();
      foreach (IdentifierExpr i in modifies) {
        oldFrameMap.Add(i.Decl, i);
      }
      Substitution oldFrameSubst = Substituter.SubstitutionFromDictionary(oldFrameMap);

      Dictionary<Block, Dictionary<Variable, Expr>> block2Incarnation = new Dictionary<Block, Dictionary<Variable, Expr>>();
      Dictionary<Variable, Expr> lastIncarnationMap = null;

      Dictionary<Variable, Expr> initialMap = new Dictionary<Variable, Expr>();

      // need to creat initial incarnation map consisting of all local, global variables -> incarnation 0


      foreach (Variable v in currentImpl.LocVars) {
        Variable incarnation = CreateIncarnation(v, Forward);
        initialMap.Add(v, new IdentifierExpr(incarnation.tok, incarnation));
      }
      foreach (Variable v in program.GlobalVariables) {
        Variable incarnation = CreateIncarnation(v, Forward);
        initialMap.Add(v, new IdentifierExpr(incarnation.tok, incarnation));
      }

      foreach (Block b in sorted) {
        Dictionary<Variable, Expr> incarnationMap = ComputeIncarnationMap(b, block2Incarnation, Forward, initialMap);
        if (Forward) {
          if (b == origToForward[loopHead]) {
            loopStartIncarnationsForward = new Dictionary<Variable, Expr>(incarnationMap);
          }
        } else {
          if (b == origToBackward[loopHead]) {
            loopStartIncarnationsBackward = new Dictionary<Variable, Expr>(incarnationMap);
          }
        }
        passifyBlock(b, incarnationMap, oldFrameSubst, Forward);
        if (b.TransferCmd is GotoCmd) {
          block2Incarnation.Add(b, incarnationMap);
        }
        lastIncarnationMap = incarnationMap;
      }
      return lastIncarnationMap;
    }

    private void passifyBlock(Block b, Dictionary<Variable, Expr> incarnationMap, Substitution oldFrameSubst, bool Forward) {
      List<Cmd> passiveCmds = new List<Cmd>();
      foreach (Cmd c in b.Cmds) {
        passifyCmd(c, incarnationMap, oldFrameSubst, passiveCmds, Forward);
      }
      b.Cmds = passiveCmds;
    }

    private void passifyCmd(Cmd c, Dictionary<Variable, Expr> incarnationMap, Substitution oldFrameSubst, List<Cmd> passiveCmds, bool Forward) {
      Substitution incarnationSubst = Substituter.SubstitutionFromDictionary(incarnationMap);
      if (c is PredicateCmd) {
        PredicateCmd pc = (PredicateCmd) c.Clone();
        pc.Expr = Substituter.ApplyReplacingOldExprs(incarnationSubst, oldFrameSubst, pc.Expr);
        passiveCmds.Add(pc);
      } else if (c is AssignCmd ac) {
        AssignCmd assign = ac.AsSimpleAssignCmd;

        // subst all variables on rhs with current incarnations
        List<Expr> assumptions = new List<Expr>();
        IDictionary<Variable, Expr> newIncarnationMappings = new Dictionary<Variable, Expr>();
        for (int i = 0; i < assign.Lhss.Count; i++) {
          Expr rhs = assign.Rhss[i];
          Expr rhsSubst = Substituter.ApplyReplacingOldExprs(incarnationSubst, oldFrameSubst, rhs);

          IdentifierExpr lhsIdExpr = ((SimpleAssignLhs)assign.Lhss[i]).AssignedVariable;
          Variable lhs = lhsIdExpr.Decl;

          if (rhs is IdentifierExpr ie) {
            if (incarnationMap.ContainsKey(ie.Decl)) {
              newIncarnationMappings[lhs] = incarnationMap[ie.Decl];
            } else {
              newIncarnationMappings[lhs] = ie;
            }
          } else {
            IdentifierExpr x_prime_expr;
            if (lhs is Incarnation) {
              x_prime_expr = lhsIdExpr;
            } else {
              Variable v = CreateIncarnation(lhs, Forward);
              x_prime_expr = new IdentifierExpr(Token.NoToken, v);
              newIncarnationMappings[lhs] = x_prime_expr;
            }

            assumptions.Add(Expr.Eq(x_prime_expr, rhsSubst));
          }
        }

        foreach (KeyValuePair<Variable, Expr> pair in newIncarnationMappings) {
          incarnationMap[pair.Key] = pair.Value;
        }
        if (assumptions.Count > 0) {
          Expr assumption = assumptions[0];
          for (int i = 1; i < assumptions.Count; i++) {
            assumption = Expr.And(assumption, assumptions[i]);
          }

          passiveCmds.Add(new AssumeCmd(Token.NoToken, assumption));
        }
      } else if (c is HavocCmd hc) {
        foreach (IdentifierExpr ie in hc.Vars) {
          Variable x = ie.Decl;
          Variable x_prime = CreateIncarnation(x, Forward);
          incarnationMap[x] = new IdentifierExpr(x_prime.tok, x_prime);
        }
        Substitution updatedIncarnationSubst = Substituter.SubstitutionFromDictionary(incarnationMap);
        foreach (IdentifierExpr ie in hc.Vars) {
          Variable x = ie.Decl;
          if (x.TypedIdent.WhereExpr != null) {
            Expr substituted = Substituter.ApplyReplacingOldExprs(updatedIncarnationSubst, oldFrameSubst, x.TypedIdent.WhereExpr);
            passiveCmds.Add(new AssumeCmd(c.tok, substituted));
          }
        }

      } else if (c is SugaredCmd sc) {
        passifyCmd(sc.Desugaring, incarnationMap, oldFrameSubst, passiveCmds, Forward);
      } else if (c is StateCmd st) {
        // local where clauses
        foreach (Variable v in st.Locals) {
          if (v.TypedIdent.WhereExpr != null) {
            passiveCmds.Add(new AssumeCmd(v.tok, v.TypedIdent.WhereExpr));
          }
        }
        foreach (Cmd s in st.Cmds) {
          passifyCmd(s, incarnationMap, oldFrameSubst, passiveCmds, Forward);
        }
        foreach (Variable v in st.Locals) {
          incarnationMap.Remove(v);
        }
      }

    }

    /*
    private Dictionary<Variable, Expr> passifyBackward(List<Block> blocks, List<IdentifierExpr> modifies) {

      Graph<Block> g = Program.GraphFromBlocks(blocks);
      IEnumerable<Block> sorted = g.TopologicalSort().Reverse();

      Dictionary<Variable, Expr> oldFrameMap = new Dictionary<Variable, Expr>();
      foreach (IdentifierExpr i in modifies) {
        oldFrameMap.Add(i.Decl, i);
      }
      Substitution oldFrameSubst = Substituter.SubstitutionFromDictionary(oldFrameMap);

      Dictionary<Block, Dictionary<Variable, Expr>> block2Incarnation = new Dictionary<Block, Dictionary<Variable, Expr>>();
      Dictionary<Variable, Expr> lastIncarnationMap = null;

      Dictionary<Variable, Expr> initialIncarnationMap = new Dictionary<Variable, Expr>();


      foreach (Block b in sorted) {
        Dictionary<Variable, Expr> incarnationMap = ComputeIncarnationMapBackward(b, block2Incarnation);
        if (b == origToBackward[loopHead]) {
          loopStartIncarnationsBackward = new Dictionary<Variable, Expr>(incarnationMap);
        }
        passifyBlockBackward(b, incarnationMap, oldFrameSubst);
        if (b.Predecessors.Count > 0) {
          block2Incarnation.Add(b, incarnationMap);
        }
        lastIncarnationMap = incarnationMap;
      }
      return lastIncarnationMap;
    }

    private void passifyBlockBackward(Block b, Dictionary<Variable, Expr> incarnationMap, Substitution oldFrameSubst) {
      List<Cmd> passiveCmds = new List<Cmd>();
      for (int i = 0; i < b.Cmds.Count; i++) {
        passifyCmdBackward(b.Cmds[i], incarnationMap, oldFrameSubst, passiveCmds);
      }
      passiveCmds.Reverse();
      b.Cmds = passiveCmds;
    }

    private void passifyCmdBackward(Cmd c, Dictionary<Variable, Expr> incarnationMap, Substitution oldFrameSubst, List<Cmd> passiveCmds) {
      Substitution incarnationSubst = Substituter.SubstitutionFromDictionary(incarnationMap);
      if (c is PredicateCmd pc) {
        pc.Expr = Substituter.ApplyReplacingOldExprs(incarnationSubst, oldFrameSubst, pc.Expr);
        passiveCmds.Add(pc);
      } else if (c is AssignCmd ac) {
        AssignCmd assign = ac.AsSimpleAssignCmd;

        // subst all variables on rhs with current incarnations
        List<Expr> assumptions = new List<Expr>();
        IDictionary<Variable, Expr> newIncarnationMappings = new Dictionary<Variable, Expr>();
        List<Expr> lhsSubsts = new List<Expr>();
        for (int i = 0; i < assign.Lhss.Count; i++) {
          IdentifierExpr lhsIdExpr = ((SimpleAssignLhs)assign.Lhss[i]).AssignedVariable;
          Variable lhs = lhsIdExpr.Decl;
          Expr lhsSubst = Substituter.ApplyReplacingOldExprs(incarnationSubst, oldFrameSubst, lhsIdExpr);
          lhsSubsts.Add(lhsSubst);

          Variable v = CreateIncarnation(lhs, false);
          IdentifierExpr x_prime_expr = new IdentifierExpr(Token.NoToken, v);
          newIncarnationMappings[lhs] = x_prime_expr;
        }

        foreach (KeyValuePair<Variable, Expr> pair in newIncarnationMappings) {
          incarnationMap[pair.Key] = pair.Value;
        }

        Substitution newIncarnationSubst = Substituter.SubstitutionFromDictionary(incarnationMap);

        for (int i = 0; i < assign.Rhss.Count; i++) {
          Expr rhs = assign.Rhss[i];
          Expr rhsSubst = Substituter.ApplyReplacingOldExprs(newIncarnationSubst, oldFrameSubst, rhs);
          assumptions.Add(Expr.Eq(lhsSubsts[i], rhsSubst));
        }

        Expr assumption = assumptions[0];
        for (int i = 1; i < assumptions.Count; i++) {
          assumption = Expr.And(assumption, assumptions[i]);
        }

        passiveCmds.Add(new AssumeCmd(Token.NoToken, assumption));

      } else if (c is HavocCmd hc) {
        foreach (IdentifierExpr ie in hc.Vars) {
          Variable x = ie.Decl;
          if (x.TypedIdent.WhereExpr != null) {
            Expr copy = Substituter.ApplyReplacingOldExprs(incarnationSubst, oldFrameSubst, x.TypedIdent.WhereExpr);
            passiveCmds.Add(new AssumeCmd(c.tok, copy));
          }
        }

        foreach (IdentifierExpr ie in hc.Vars) {
          Variable x = ie.Decl;
          Variable x_prime = CreateIncarnation(x, false);
          incarnationMap[x] = new IdentifierExpr(x_prime.tok, x_prime);
        }
      } else if (c is SugaredCmd sc) {
        passifyCmdBackward(sc.Desugaring, incarnationMap, oldFrameSubst, passiveCmds);
      } else if (c is StateCmd st) {
        // local where clauses
        foreach (Variable v in st.Locals) {
          if (v.TypedIdent.WhereExpr != null) {
            passiveCmds.Add(new AssumeCmd(v.tok, v.TypedIdent.WhereExpr));
          }
        }
        for (int i = 0; i < st.Cmds.Count; i++) {
          passifyCmdBackward(st.Cmds[i], incarnationMap, oldFrameSubst, passiveCmds);
        }
        foreach (Variable v in st.Locals) {
          incarnationMap.Remove(v);
        }
      }

    }
    */

    private Variable CreateIncarnation(Variable x, bool Forward) {
      int incarnationNumber;
      if (Forward) {
        if (varToIncarnationForward.ContainsKey(x)) {
          varToIncarnationForward[x] = varToIncarnationForward[x] + 1;
        } else {
          varToIncarnationForward[x] = 0;
        }
        incarnationNumber = varToIncarnationForward[x];
      } else {
        if (varToIncarnationBackward.ContainsKey(x)) {
          varToIncarnationBackward[x] = varToIncarnationBackward[x] + 1;
        } else {
          varToIncarnationBackward[x] = 0;
        }
        incarnationNumber = varToIncarnationBackward[x];
      }
      Variable xPrime = new Incarnation(x, incarnationNumber, Forward);
      return xPrime;
    }

    private void InsertPostCondition(IEnumerable<Block> blocks) {

      List<Block> exits = new List<Block>();
      foreach (Block b in blocks) {
        if (b.TransferCmd is ReturnCmd) {
          exits.Add(b);
        }
      }
      Block exitBlock = exits[0];
      if (exits.Count > 1) {
        string unifiedExitLabel = "UnifiedExit";
        exitBlock = new Block(Token.NoToken, unifiedExitLabel, new List<Cmd>(),
          new ReturnCmd(Token.NoToken));
        foreach (Block exit in exits) {
          exit.TransferCmd = new GotoCmd(Token.NoToken, new List<Block>() { exitBlock });
          exitBlock.Predecessors.Add(exit);
        }
        impl.Blocks.Add(exitBlock);
      }
      List<Cmd> postconditions = new List<Cmd>();
      Substitution formalProcImplSubst = Substituter.SubstitutionFromDictionary(impl.GetImplFormalMap());

      // add requires clauses
      foreach (Ensures ens in impl.Proc.Ensures) {
        Expr e = Substituter.Apply(formalProcImplSubst, ens.Condition);
        Cmd c = new AssertCmd(ens.tok, e);
        postconditions.Add(c);
      }

      exitBlock.Cmds.AddRange(postconditions);
    }

    private void InsertPreCondition(Block start) {
      List<Cmd> preconditions = new List<Cmd>();

      Substitution formalProcImplSubst = Substituter.SubstitutionFromDictionary(impl.GetImplFormalMap());

      // add global variable where clauses
      foreach (Variable global in program.GlobalVariables) {
        if (global.TypedIdent.WhereExpr != null) {
          preconditions.Add(new AssumeCmd(global.tok, global.TypedIdent.WhereExpr));
        }
      }

      // in-parameter where clauses
      foreach (Formal f in impl.Proc.InParams) {
        if (f.TypedIdent.WhereExpr != null) {
          Expr e = Substituter.Apply(formalProcImplSubst, f.TypedIdent.WhereExpr);
          Cmd c = new AssumeCmd(f.tok, e);
          preconditions.Add(c);
        }
      }
      
      // out-parameter where clauses
      foreach (Variable f in impl.Proc.OutParams) {
        if (f.TypedIdent.WhereExpr != null) {
          Expr e = Substituter.Apply(formalProcImplSubst, f.TypedIdent.WhereExpr);
          Cmd c = new AssumeCmd(f.tok, e);
          preconditions.Add(c);
        }
      }

      // add local variable where clauses
      foreach (Variable local in impl.LocVars) {
        if (local.TypedIdent.WhereExpr != null) {
          preconditions.Add(new AssumeCmd(local.tok, local.TypedIdent.WhereExpr));
        }
      }

      // add requires clauses
      foreach (Requires req in impl.Proc.Requires) {
        Expr e = Substituter.Apply(formalProcImplSubst, req.Condition);
        Cmd c = new AssumeCmd(req.tok, e);
        preconditions.Add(c);
      }

      // add preconditions to start of start block
      preconditions.AddRange(start.cmds);
      start.cmds = preconditions;
    }

  }
}
