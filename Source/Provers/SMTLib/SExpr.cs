using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.Contracts;
using Microsoft.Boogie.VCExprAST;
using Microsoft.BaseTypes;

namespace Microsoft.Boogie
{
  public class SExpr
  {
    static readonly SExpr[] EmptyArgs = new SExpr[0];
    public readonly string Name;

    public SExpr[] Arguments
    {
      get
      {
        Contract.Ensures(Contract.Result<SExpr[]>() != null);
        Contract.Ensures(Contract.ForAll(Contract.Result<SExpr[]>(), expr => expr != null));

        return this.arguments;
      }
    }

    public SExpr this[int idx]
    {
      get { return Arguments[idx]; }
    }

    public int ArgCount
    {
      get { return arguments.Length; }
    }

    public bool IsId
    {
      get { return Arguments.Length == 0; }
    }

    private readonly SExpr[] arguments;

    [ContractInvariantMethod]
    void ObjectInvariant()
    {
      Contract.Invariant(this.Name != null);
      Contract.Invariant(this.arguments != null);
      Contract.Invariant(Contract.ForAll(this.arguments, arg => arg != null));
    }

    public SExpr(string name, params SExpr[] args)
      : this(name, (IEnumerable<SExpr>) args)
    {
      Contract.Requires(name != null);
      Contract.Requires(args != null);
      Contract.Requires(Contract.ForAll(args, x => x != null));
    }

    public SExpr(string name, IEnumerable<SExpr> args)
    {
      Contract.Requires(name != null);
      Contract.Requires(args != null);
      // We don't want to evaluate args twice!
      // Contract.Requires(Contract.ForAll(args, x => x != null));
      Name = name;
      arguments = args.ToArray();
    }

    public SExpr(string name)
      : this(name, EmptyArgs)
    {
      Contract.Requires(name != null);
    }

    #region pretty-printing

    void WriteTo(StringBuilder sb)
    {
      Contract.Requires(sb != null);

      if (Arguments.Length > 0)
      {
        sb.Append('(');
      }

      if (Name.Any(Char.IsWhiteSpace))
      {
        sb.Append("\"").Append(Name).Append("\"");
      }
      else if (Name.Length == 0)
      {
        // this was not an accurate representation
        //sb.Append("()");
      }
      else
      {
        sb.Append(Name);
      }

      foreach (var a in Arguments)
      {
        sb.Append(' ');
        a.WriteTo(sb);
      }

      if (Arguments.Length > 0)
      {
        sb.Append(')');
      }
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      this.WriteTo(sb);
      return sb.ToString();
    }

    #endregion

    #region parsing

    public abstract class Parser
    {
      System.IO.StreamReader sr;
      int linePos = 0;
      string currLine = null;

      public Parser(System.IO.StreamReader _sr)
      {
        sr = _sr;
      }

      string Read()
      {
        return sr.ReadLine();
      }

      char SkipWs()
      {
        while (true)
        {
          if (currLine == null)
          {
            currLine = Read();
            if (currLine == null)
            {
              return '\0';
            }
          }

          while (linePos < currLine.Length && char.IsWhiteSpace(currLine[linePos]))
          {
            linePos++;
          }

          if (linePos < currLine.Length)
          {
            return currLine[linePos];
          }
          else
          {
            currLine = null;
            linePos = 0;
          }
        }
      }

      void Shift()
      {
        linePos++;
      }

      string ParseId()
      {
        var sb = new StringBuilder();

        var beg = SkipWs();

        var quoted = beg == '"' || beg == '|';
        if (quoted)
        {
          Shift();
        }

        while (true)
        {
          if (linePos >= currLine.Length)
          {
            if (quoted)
            {
              sb.Append("\n");
              currLine = Read();
              linePos = 0;
              if (currLine == null)
              {
                break;
              }
            }
            else
            {
              break;
            }
          }

          var c = currLine[linePos++];
          if (quoted && c == beg)
          {
            break;
          }

          if (!quoted && (char.IsWhiteSpace(c) || c == '(' || c == ')'))
          {
            linePos--;
            break;
          }

          if (quoted && c == '\\' && linePos < currLine.Length && currLine[linePos] == '"')
          {
            sb.Append('"');
            linePos++;
            continue;
          }

          sb.Append(c);
        }

        return sb.ToString();
      }

      public abstract void ParseError(string msg);

      public IEnumerable<SExpr> ParseSExprs(bool top)
      {
        while (true)
        {
          var c = SkipWs();
          if (c == '\0')
          {
            break;
          }

          if (c == ')')
          {
            if (top)
            {
              ParseError("stray ')'");
            }

            break;
          }

          string id;

          if (c == '(')
          {
            Shift();
            c = SkipWs();
            if (c == '\0')
            {
              ParseError("expecting something after '('");
              break;
            }
            else if (c == '(')
            {
              id = "";
            }
            else
            {
              id = ParseId();
            }

            var args = ParseSExprs(false).ToArray();

            c = SkipWs();
            if (c == ')')
            {
              Shift();
            }
            else
            {
              ParseError("unclosed '(" + id + "'");
            }

            yield return new SExpr(id, args);
          }
          else
          {
            id = ParseId();
            yield return new SExpr(id);
          }

          if (top)
          {
            break;
          }
        }
      }
    }

    #endregion

    // really need to put this somewhere else 
    public static SExpr ResolveLet(SExpr sexpr, Dictionary<String, SExpr> letDefs) {
      SExpr toReplace;
      if (letDefs.TryGetValue(sexpr.Name, out toReplace)) {
        // find symbol to replace
        return toReplace;
      } else if (sexpr.Name == "let") {
        // find new let expression
        foreach (SExpr def in sexpr[0].Arguments) {
          letDefs.Add(def.Name, ResolveLet(def[0], letDefs));
        }
        SExpr resolved = ResolveLet(sexpr[1], letDefs);
        return resolved;
      }
      // resolve all arguments
      List<SExpr> newArgs = new List<SExpr>();
      for (int i = 0; i < sexpr.ArgCount; i++) {
        SExpr s = sexpr.Arguments[i];
        newArgs.Add(ResolveLet(s, letDefs));
      }
      return new SExpr(sexpr.Name, newArgs);
    }

    public VCExpr ToVC(VCExpressionGenerator gen, Boogie2VCExprTranslator translator, IEnumerable<Variable> scopeVars, Dictionary<String, VCExprVar> boundVars) {
      // still need to add floats
      if (Name == "let") {
        List<VCExprLetBinding> bindings = new List<VCExprLetBinding>();
        Dictionary<String, VCExprVar> boundVarsUpdate = new Dictionary<String, VCExprVar>(boundVars);
        foreach (SExpr def in Arguments[0].Arguments) {
          VCExpr e = def.Arguments[0].ToVC(gen, translator, scopeVars, boundVars);
          VCExprVar bound = gen.Variable(def.Name, e.Type);
          boundVarsUpdate.Add(def.Name, bound);
          bindings.Add(gen.LetBinding(bound, e));
        }

        VCExpr body = Arguments[1].ToVC(gen, translator, scopeVars, boundVarsUpdate);

        return gen.Let(bindings, body);
      }

      List<VCExpr> args = new List<VCExpr>();
      foreach (SExpr arg in Arguments) {
        args.Add(arg.ToVC(gen, translator, scopeVars, boundVars));
      }
      VCExpr combinedArgs;
      switch (Name) {
        case "":
          if (args.Count == 1) {
            return args[0];
          }
          break;
        case "and":
          combinedArgs = gen.AndSimp(args[0], args[1]);
          for (int i = 2; i < args.Count; i++) {
            combinedArgs = gen.AndSimp(combinedArgs, args[i]);
          }
          return combinedArgs;
        case "or":
          combinedArgs = gen.OrSimp(args[0], args[1]);
          for (int i = 2; i < args.Count; i++) {
            combinedArgs = gen.OrSimp(combinedArgs, args[i]);
          }
          return combinedArgs;
          break;
        case "not":
          return gen.NotSimp(args[0]);
        case "=>":
          return gen.ImpliesSimp(args[0], args[1]);
        case "+":
          if (args[0].Type.IsInt) {
            combinedArgs = gen.Function(VCExpressionGenerator.AddIOp, new List<VCExpr> { args[0], args[1] });
            for (int i = 2; i < args.Count; i++) {
              combinedArgs = gen.Function(VCExpressionGenerator.AddIOp, new List<VCExpr> { combinedArgs, args[i] });
            }
            return combinedArgs;
          } else if (args[0].Type.IsReal) {
            combinedArgs = gen.Function(VCExpressionGenerator.AddROp, new List<VCExpr> { args[0], args[1] });
            for (int i = 2; i < args.Count; i++) {
              combinedArgs = gen.Function(VCExpressionGenerator.AddROp, new List<VCExpr> { combinedArgs, args[i] });
            }
            return combinedArgs;
          }
          break;
        case "-":
          if (ArgCount == 1) {
            if (args[0] is VCExprIntLit) {
              BigNum val = ((VCExprIntLit)args[0]).Val;
              return gen.Integer(-val);
            } else if (args[0] is VCExprRealLit) {
              BigDec val = ((VCExprRealLit)args[0]).Val;
              return gen.Real(-val);
            } else {
              if (args[0].Type.IsInt) {
                return gen.Function(VCExpressionGenerator.SubIOp, gen.Integer(BigNum.ZERO), args[0]);
              } else if (args[0].Type.IsReal) {
                return gen.Function(VCExpressionGenerator.SubROp, gen.Real(BigDec.ZERO), args[0]);
              }
            }
          } else if (ArgCount == 2) {
            if (args[0].Type.IsInt) {
              return gen.Function(VCExpressionGenerator.SubIOp, args);
            } else if (args[0].Type.IsReal) {
              return gen.Function(VCExpressionGenerator.SubROp, args);
            }
          }
          break;
        case "*":
          if (args[0].Type.IsInt) {
            combinedArgs = gen.Function(VCExpressionGenerator.MulIOp, new List<VCExpr> { args[0], args[1] });
            for (int i = 2; i < args.Count; i++) {
              combinedArgs = gen.Function(VCExpressionGenerator.MulIOp, new List<VCExpr> { combinedArgs, args[i] });
            }
            return combinedArgs;
          } else if (args[0].Type.IsReal) {
            combinedArgs = gen.Function(VCExpressionGenerator.MulROp, new List<VCExpr> { args[0], args[1] });
            for (int i = 2; i < args.Count; i++) {
              combinedArgs = gen.Function(VCExpressionGenerator.MulROp, new List<VCExpr> { combinedArgs, args[i] });
            }
            return combinedArgs;
          }
          break;
        case "div":
          return gen.Function(VCExpressionGenerator.DivIOp, args);
        case "/":
          return gen.Function(VCExpressionGenerator.DivROp, args);
        case "mod":
          return gen.Function(VCExpressionGenerator.ModOp, args);
        case "=":
          return gen.Function(VCExpressionGenerator.EqOp, args);
        case ">":
          return gen.Function(VCExpressionGenerator.GtOp, args);
        case "<":
          return gen.Function(VCExpressionGenerator.LtOp, args);
        case "<=":
          return gen.Function(VCExpressionGenerator.LeOp, args);
        case ">=":
          return gen.Function(VCExpressionGenerator.GeOp, args);
        case "ite":
          return gen.Function(VCExpressionGenerator.IfThenElseOp, args);
        case "true":
          return VCExpressionGenerator.True;
        case "false":
          return VCExpressionGenerator.False;
        case "to_int":
          return gen.Function(VCExpressionGenerator.ToIntOp, args);
        case "to_real":
          return gen.Function(VCExpressionGenerator.ToRealOp, args);
        default:
          BigNum num;
          BigDec dec;
          if (Name.All(Char.IsDigit)) {
            if (BigNum.TryParse(Name, out num)) {
              // int
              return gen.Integer(num);
            }
          }
          if (Name.All(c => Char.IsDigit(c) || c == '.' || c == 'e')) {
            if (BigDec.TryParse(Name, out dec)) {
              // real
              return gen.Real(dec);
            }
          }
          // identifier
          foreach (Variable v in scopeVars) {
            if (v.Name.Equals(Name)) {
              return translator.LookupVariable(v);
            }
          }
          foreach (KeyValuePair<String, VCExprVar> kv in boundVars) {
            if (kv.Key.Equals(Name)) {
              return kv.Value;
            }
          }
          // can figure out floats later
          break;
      }
      throw new NotImplementedException("unimplemented for conversion to VCExpr: " + this);
    }
  }

}