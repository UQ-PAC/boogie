using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Boogie.VCExprAST;

namespace Microsoft.Boogie.SMTLib {
  public class MathSAT : SMTLibProcessTheoremProver {
    private MathSAT(SMTLibOptions libOptions, ProverOptions options, VCExpressionGenerator gen, SMTLibProverContext ctx) : base(libOptions, options, gen, ctx) {

    }

    public static MathSAT CreateProver(Program prog, string /*?*/ logFilePath, bool appendLogFile, uint timeout) {
      ProverOptions options = cce.NonNull(CommandLineOptions.Clo.TheProverFactory).BlankProverOptions();

      if (logFilePath != null) {
        options.LogFilename = logFilePath;
        if (appendLogFile) {
          options.AppendLogFile = appendLogFile;
        }
      }

      if (timeout > 0) {
        options.TimeLimit = Util.BoundedMultiply(timeout, 1000);
      }
      options.Parse(CommandLineOptions.Clo.ProverOptions);

      SMTLibOptions libOptions = CommandLineOptions.Clo;

      // we want to override the options intended for the main prover with prover options specific to MathSAT, do so here?


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

    public VCExpr computeInterpolant(VCExpr A, VCExpr B) {

      // push?
      SendThisVC("(push 1)");

      // need to add axioms??

      // declare A & B as functions
      SendThisVC("(define-fun A () Bool (" + VCExpr2String(A, 1) + ")"); // need to check what VCExpr2String polarity actually does
      SendThisVC("(define-fun B () Bool (" + VCExpr2String(B, 1) + ")");

      // define interpolation groups?
      SendThisVC("(assert (! A :interpolation-group g1))");
      SendThisVC("(assert (! A :interpolation-group g2))");

      // request interpolant
      SendCheckSat();
      SendThisVC("(get-interpolant (g1)");

      // get response & parse (just smt-lib expression)

      // pop?
      SendThisVC("(push 1)");

    }

  }
}
