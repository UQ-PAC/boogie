using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Boogie.VCExprAST;

namespace Microsoft.Boogie.SMTLib {
  public class MathSAT : SMTLibProcessTheoremProver {
    private MathSAT(SMTLibOptions libOptions, ProverOptions options, VCExpressionGenerator gen, SMTLibProverContext ctx) : base(libOptions, options, gen, ctx) {

    }

    public static MathSAT CreateProver(Program prog, string /*?*/ logFilePath, bool appendLogFile, uint timeout) {
      ProverOptions options = new MathSATOptions();

      if (logFilePath != null) {
        options.LogFilename = logFilePath;
        if (appendLogFile) {
          options.AppendLogFile = appendLogFile;
        }
      }

      if (timeout > 0) {
        options.TimeLimit = Util.BoundedMultiply(timeout, 1000);
      }

      SMTLibOptions libOptions = CommandLineOptions.Clo; // not sure if need to override anything here specifically for MathSAT?

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

      return new MathSAT(libOptions, options, ctx.ExprGen, ctx);
    }

    public bool Satisfiable(VCExpr A, VCExpr B, string AStr, string BStr) {
      string AStr2;
      string BStr2;
      InterpolationSetup("interpolant", A, B, out AStr2, out BStr2);
      if (AStr == "") {
        AStr = AStr2;
      }
      if (BStr == "") {
        BStr = BStr2;
      }

      SendThisVC("(push 1)");

      // declare A & B as functions
      SendThisVC("(define-fun A () Bool " + AStr + ")");
      SendThisVC("(define-fun B () Bool " + BStr + ")");

      // define interpolation groups
      SendThisVC("(assert (! A :interpolation-group g1))");
      SendThisVC("(assert (! B :interpolation-group g2))");
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
    public SExpr CalculateInterpolant() {
      SendThisVC("(get-interpolant (g1))");
      SExpr resp = Process.GetProverResponse();
      //Console.WriteLine("interpolant: " + resp.ToString());
      SendThisVC("(pop 1)"); 
      FlushLogFile();

      Dictionary<String, SExpr> letDefs = new Dictionary<String, SExpr>();
      return SExpr.ResolveLet(resp, letDefs);
    }

  }

  public class MathSATOptions : SMTLibProverOptions {

    public MathSATOptions() {
      this.Solver = SolverKind.MATHSAT;
      SolverArguments.Add("-input=smt2");
      SolverArguments.Add("-interpolation=TRUE");
      ProverName = "mathsat";
    }

  }
}
