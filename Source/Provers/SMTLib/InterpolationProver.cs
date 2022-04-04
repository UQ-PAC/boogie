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

      switch (CommandLineOptions.Clo.InterpolantSolverKind) {
        case CommandLineOptions.InterpolantSolver.MathSAT:
          options = new MathSATOptions();
          break;
        case CommandLineOptions.InterpolantSolver.SMTInterpol:
          options = new SMTInterpolOptions();
          break;
        case CommandLineOptions.InterpolantSolver.Princess:
          options = new PrincessOptions();
          break;
        case CommandLineOptions.InterpolantSolver.CVC5:
          options = new CVC5Options();
          break;
        default:
          throw new cce.UnreachableException();
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

      // do not want axioms getting in the way of interpolation problem, add them directly later
      /*
      foreach (var ax in prog.Axioms) {
        ctx.AddAxiom(ax, null);
      }
      */

      foreach (Declaration decl in prog.TopLevelDeclarations) {
        GlobalVariable v = decl as GlobalVariable;
        if (v != null) {
          ctx.DeclareGlobalVariable(v, null);
        }
      }

      return new InterpolationProver(libOptions, options, ctx.ExprGen, ctx);
    }
    public void InterpolationSetup(string descriptiveName, VCExpr A, VCExpr B, out string AStr, out string BStr) {
      if (options.SeparateLogFiles) {
        CloseLogFile(); // shouldn't really happen
      }

      if (options.LogFilename != null && currentLogFile == null) {
        currentLogFile = OpenOutputFile(descriptiveName);
        currentLogFile.Write(common.ToString());
      }

      PrepareCommon();
      FlushAndCacheCommons();
      AStr = VCExpr2String(A, 1);
      BStr = VCExpr2String(B, 1);
      FlushAxioms();
    }

    // returns false if A && B is satisfiable meaning interpolant can't be found
    // returns true and sets resp to be output from smt solver if interpolant is found
    public bool CalculateInterpolant(VCExpr A, VCExpr B, bool Forward, out VCExpr I, SortedDictionary<(string, int), Function> bvOps, List<Function> newBVFunctions, Dictionary<Function, VCExpr> functionDefs, List<VCExpr> QFAxioms) {
      Stopwatch stopWatch = new Stopwatch();
      if (CommandLineOptions.Clo.InterpolationProfiling) {
        stopWatch.Start();
      }

      List<Function> functions = functionDefs.Keys.ToList();

      Dictionary<Function, List<List<VCExpr>>> toInstantiateA = FunctionParametersCollector.Collect(A, functions);
      Dictionary<Function, List<List<VCExpr>>> toInstantiateB = FunctionParametersCollector.Collect(B, functions);

      foreach (Function f in functions) {
        List<List<VCExpr>> parameterLists;
        VCExprQuantifier quantifier = functionDefs[f] as VCExprQuantifier;

        if (toInstantiateA.TryGetValue(f, out parameterLists)) {
          foreach (List<VCExpr> parameters in parameterLists) {
            Dictionary<VCExprVar, VCExpr> toSubst = new Dictionary<VCExprVar, VCExpr>();
            for (int i = 0; i < quantifier.BoundVars.Count; i++) {
              toSubst.Add(quantifier.BoundVars[i], parameters[i]);
            }
            VCExprSubstitution subst = new VCExprSubstitution(toSubst, new Dictionary<TypeVariable, Type>());
            SubstitutingVCExprVisitor substituter = new SubstitutingVCExprVisitor(gen);
            VCExpr instantiated = substituter.Mutate(quantifier.Body, subst);
            A = gen.AndSimp(instantiated, A);
          }
        }

        if (toInstantiateB.TryGetValue(f, out parameterLists)) {
          foreach (List<VCExpr> parameters in parameterLists) {
            Dictionary<VCExprVar, VCExpr> toSubst = new Dictionary<VCExprVar, VCExpr>();
            for (int i = 0; i < quantifier.BoundVars.Count; i++) {
              toSubst.Add(quantifier.BoundVars[i], parameters[i]);
            }
            VCExprSubstitution subst = new VCExprSubstitution(toSubst, new Dictionary<TypeVariable, Type>());
            SubstitutingVCExprVisitor substituter = new SubstitutingVCExprVisitor(gen);
            VCExpr instantiated = substituter.Mutate(quantifier.Body, subst);
            B = gen.AndSimp(instantiated, B);
          }
        }

      }

      // add quantifier-free axioms to both sides
      foreach (VCExpr axiom in QFAxioms) {
        A = gen.AndSimp(axiom, A);
        B = gen.AndSimp(axiom, B);
      }

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
      } else if (options.Solver == SolverKind.PRINCESS) {
        SendThisVC("(assert " + AStr + ")");
        SendThisVC("(assert " + BStr + ")");
      } else if (options.Solver == SolverKind.CVC5) {
        SendThisVC("(assert " + AStr + ")");
        SendThisVC("(push 1)");
        SendThisVC("(assert " + BStr + ")");
      }
      // need to check sat before requesting interpolant
      SendCheckSat();
      SExpr satResp = Process.GetProverResponse();
      if (CommandLineOptions.Clo.InterpolationProfiling) {
        stopWatch.Stop();
        Console.WriteLine("interpol sat time: " + String.Format("{0:N3}", stopWatch.Elapsed.TotalSeconds));
        stopWatch.Reset();
        stopWatch.Start();
      }

      if (options.Solver == SolverKind.CVC5) {
        SendThisVC("(pop 1)");
      }

      if (satResp.Name == "sat") {
        SendThisVC("(pop 1)");
        FlushLogFile();
        I = null;
        if (CommandLineOptions.Clo.InterpolationProfiling) {
          stopWatch.Stop();
        }
        return false;
      } else if (satResp.Name != "unsat") {
        SendThisVC("(pop 1)");
        FlushLogFile();
        throw new ProverException("unexpected prover response " + satResp);
      }

      // A && B is unsat, find interpolant
      if (options.Solver == SolverKind.MATHSAT) {
        if (Forward) {
          SendThisVC("(get-interpolant (g2))");
        } else {
          SendThisVC("(get-interpolant (g1))");
        }
      } else if (options.Solver == SolverKind.SMTINTERPOL) {
        if (Forward) {
          SendThisVC("(get-interpolants g2 g1)");
        } else {
          SendThisVC("(get-interpolants g1 g2)");
        }
      } else if (options.Solver == SolverKind.CVC5) {
        // unlike other provers, CVC5 does not compute reverse interpolants as its default interpolation procedure so must negate B to get desired result
        SendThisVC("(get-interpol g1 (not" + BStr + "))");
      } else if (options.Solver == SolverKind.PRINCESS) {
        SendThisVC("(get-interpolants)");
      }

      SExpr resp = Process.GetProverResponse();
      if (CommandLineOptions.Clo.InterpolationProfiling) {
        stopWatch.Stop();
        Console.WriteLine("total interpol time: " + String.Format("{0:N3}", stopWatch.Elapsed.TotalSeconds));
      }
      //Console.WriteLine("interpolant: " + resp.ToString());
      if (options.Solver == SolverKind.CVC5) {
        // CVC5 adds extraneous function definition stuff like this
        resp = resp.Arguments[3];
      }
      SendThisVC("(pop 1)");
      FlushLogFile();
      I = resp.ToVC(gen, ctx.BoogieExprTranslator, bvOps, newBVFunctions, Namer);

      //Dictionary<String, SExpr> letDefs = new Dictionary<String, SExpr>();
      //return SExpr.ResolveLet(resp, letDefs);
      return true;
    }

  }

  public class MathSATOptions : SMTLibProverOptions {

    public MathSATOptions() {
      this.Solver = SolverKind.MATHSAT;
      SolverArguments.Add("-input=smt2 -theory.bv.eager=false");
      SolverArguments.Add("-theory.bv.interpolation_mode=" + CommandLineOptions.Clo.InterpolationBVMode);
      ProverName = "mathsat";
    }

  }

  public class SMTInterpolOptions : SMTLibProverOptions {

    public SMTInterpolOptions() {
      this.Solver = SolverKind.SMTINTERPOL;
      SolverArguments.Add("-q");
      ProverName = "smtinterpol.jar";
      Logic = "QF_UFLIRA";
    }

  }

  public class CVC5Options : SMTLibProverOptions {
    public CVC5Options() {
      this.Solver = SolverKind.CVC5;
      SolverArguments.Add("--produce-interpols=default");
      SolverArguments.Add("--incremental");
      ProverName = "cvc5";
      Logic = "ALL";
    }

  }

  public class PrincessOptions : SMTLibProverOptions {
    public PrincessOptions() {
      this.Solver = SolverKind.PRINCESS;
      ProverName = "princess-all.jar";
      SolverArguments.Add("ap.CmdlMain");
      SolverArguments.Add("-logo");
      SolverArguments.Add("+quiet");
      SolverArguments.Add("+stdin");
      SolverArguments.Add("+incremental");
      SolverArguments.Add("+elimInterpolantQuants");
    }

  }

  public class FunctionParametersCollector : TraversingVCExprVisitor<bool, bool> {

    private List<Function> functions;
    private Dictionary<Function, List<List<VCExpr>>> functionParameters;

    public FunctionParametersCollector(List<Function> functions) {
      this.functions = functions;
      this.functionParameters = new Dictionary<Function, List<List<VCExpr>>>();
    }

    public static Dictionary<Function, List<List<VCExpr>>> Collect(VCExpr expr, List<Function> functions) {
      FunctionParametersCollector visitor = new FunctionParametersCollector(functions);
      visitor.Traverse(expr, true);
      return visitor.functionParameters;
    }

    public override bool Visit(VCExprNAry node, bool arg) {
      if (node.Op is VCExprBoogieFunctionOp) {
        VCExprBoogieFunctionOp op = node.Op as VCExprBoogieFunctionOp;
        if (functions.Contains(op.Func)) {
          List<VCExpr> args = new List<VCExpr>();
          foreach (VCExpr e in node.UniformArguments) {
            args.Add(e);
          }
          List<List<VCExpr>> collectedParams;
          if (functionParameters.TryGetValue(op.Func, out collectedParams)) {
            collectedParams.Add(args);
          } else {
            functionParameters.Add(op.Func, new List<List<VCExpr>> { args });
          }
        }
      }
      return base.Visit(node, arg);
    }

    protected override bool StandardResult(VCExpr node, bool arg) {
      return true;
    }
  }

}

