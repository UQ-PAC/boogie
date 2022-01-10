using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Boogie.VCExprAST;

namespace Microsoft.Boogie.SMTLib {
  public class InterpolationProver : SMTLibProcessTheoremProver {
    private InterpolationProver(SMTLibOptions libOptions, ProverOptions options, VCExpressionGenerator gen, SMTLibProverContext ctx) : base(libOptions, options, gen, ctx) {

    }

    public static InterpolationProver CreateProver(Program prog, string /*?*/ logFilePath, bool appendLogFile, uint timeout) {
      ProverOptions options;
      if (CommandLineOptions.Clo.InterpolantSolverKind == CommandLineOptions.InterpolantSolver.MathSAT) {
        options = new MathSATOptions();
      } else {
        options = new SMTInterpolOptions();
      }

      if (logFilePath != null) {
        options.LogFilename = logFilePath;
        if (appendLogFile) {
          options.AppendLogFile = appendLogFile;
        }
      }

      if (timeout > 0) {
        options.TimeLimit = Util.BoundedMultiply(timeout, 1000);
      }

      SMTLibOptions libOptions = CommandLineOptions.Clo;

      VCExpressionGenerator gen = new VCExpressionGenerator();

      List<string> proverCommands = new List<string>();
      proverCommands.Add("smtlib");
      proverCommands.Add("external");
      VCGenerationOptions genOptions = new VCGenerationOptions(proverCommands);

      SMTLibProverContext ctx = new SMTLibProverContext(gen, genOptions);

      // set up the context
      foreach (Declaration decl in prog.TopLevelDeclarations) {
        TypeCtorDecl t = decl as TypeCtorDecl;
        if (t != null) {
          ctx.DeclareType(t, null);
        }
      }

      foreach (Declaration decl in prog.TopLevelDeclarations) {
        Constant c = decl as Constant;
        if (c != null) {
          ctx.DeclareConstant(c, c.Unique, null);
        } else {
          Function f = decl as Function;
          if (f != null) {
            ctx.DeclareFunction(f, null);
          }
        }
      }

      foreach (var ax in prog.Axioms) {
        ctx.AddAxiom(ax, null);
      }

      foreach (Declaration decl in prog.TopLevelDeclarations) {
        GlobalVariable v = decl as GlobalVariable;
        if (v != null) {
          ctx.DeclareGlobalVariable(v, null);
        }
      }

      return new InterpolationProver(libOptions, options, ctx.ExprGen, ctx);
    }

    public bool Satisfiable(VCExpr A, VCExpr B) {
      string AStr;
      string BStr;
      InterpolationSetup("interpolant", A, B, out AStr, out BStr);

      SendThisVC("(push 1)");

      if (options.Solver == SolverKind.SMTINTERPOL) {
        SendThisVC("(set-option :" + Z3.TimeoutOption + " " + options.TimeLimit + ")");
      }

      // define interpolation groups
      if (options.Solver == SolverKind.MATHSAT) {
        SendThisVC("(assert (! " + AStr + " :interpolation-group g1))");
        SendThisVC("(assert (! " + BStr + " :interpolation-group g2))");
      } else if (options.Solver == SolverKind.SMTINTERPOL) {
        SendThisVC("(assert (! " + AStr + " :named g1))");
        SendThisVC("(assert (! " + BStr + " :named g2))");
      }
      // need to check sat before requesting interpolant
      SendCheckSat();
      SExpr resp = Process.GetProverResponse();
      Debug.Print(resp.ToString());

      if (resp.Name == "sat") {
        SendThisVC("(pop 1)");
        FlushLogFile();
        return true;
      } else if (resp.Name != "unsat") {
        SendThisVC("(pop 1)");
        FlushLogFile();
        throw new ProverException("unexpected prover response " + resp);
      }
      return false;
    }

    // must have called Satisfiable first
    public SExpr CalculateInterpolant(bool Forward) {
      if (options.Solver == SolverKind.MATHSAT) {
        if (Forward) {
          SendThisVC("(get-interpolant (g2))");
        } else {
          SendThisVC("(get-interpolant (g1))");
        }
      } else if (options.Solver == SolverKind.SMTINTERPOL) {
        if (Forward) {
          SendThisVC("(get-interpolants g2 g1)");
        }
        else {
          SendThisVC("(get-interpolants g1 g2)");
        }
      }

      SExpr resp = Process.GetProverResponse();
      //Console.WriteLine("interpolant: " + resp.ToString());
      SendThisVC("(pop 1)"); 
      FlushLogFile();

      //Dictionary<String, SExpr> letDefs = new Dictionary<String, SExpr>();
      //return SExpr.ResolveLet(resp, letDefs);
      return resp;
    }

  }

  public class MathSATOptions : SMTLibProverOptions {

    public MathSATOptions() {
      this.Solver = SolverKind.MATHSAT;
      SolverArguments.Add("-input=smt2");
      ProverName = "mathsat";
    }

  }

  public class SMTInterpolOptions : SMTLibProverOptions {

    public SMTInterpolOptions() {
      this.Solver = SolverKind.SMTINTERPOL;
      SolverArguments.Add("-q");
      ProverName = "smtinterpol";
      Logic = "QF_UFLIRA";
    }

  }

}

