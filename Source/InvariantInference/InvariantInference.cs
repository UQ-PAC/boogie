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
                  InferLoopInvariant(program, procedureImplementations, impl, b);
                }
              }
            }
          }
        }
      }
    }

    private static void InferLoopInvariant(Program program, Dictionary<Procedure, Implementation[]> procImpl, Implementation impl, Block loopHead) {
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

    private static void getPostCondition(Block end, Block loopHead) {
      List<List<Block>> paths;

    }

    private static void getPreCondition(Block start, Block loopHead) {
      List<List<Block>> paths;
    }


    private static VCExpr weakestPrecondition(Cmd cmd, VCExpr Q) {
      /*
      VCExpressionGenerator gen = new VCExpressionGenerator();
      if (cmd is AssertCmd) {
        return gen.AndSimp()
      } else if (cmd is AssumeCmd) {
        return gen.ImpliesSimp()
      } else if (cmd is HavocCmd) {

      } else if (cmd is AssignCmd) {

      } else {
        Debug.Print("error: unimplemented command for WP: " + cmd.ToString());
      } */
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

