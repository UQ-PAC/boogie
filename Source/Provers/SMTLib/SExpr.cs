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

    /*
    public VCExpr ToVCDisambig(VCExpressionGenerator gen, Boogie2VCExprTranslator translator, IEnumerable<Variable> scopeVars) {
      VCExpr a = ToVC(gen, translator, scopeVars, new Dictionary<string, VCExprVar>());
      VCExpr b = ToVC(gen, translator, scopeVars);

      if (a.ToString() != b.ToString()) {
        Console.WriteLine("sexpr conversion difference!!!");
        Console.WriteLine(a);
        Console.WriteLine(b);
        Console.Out.Flush();
        throw new Exception("fafsdfae");
      }

      if (CommandLineOptions.Clo.oldVCConvert) {
        return a;
      } else {
        return b;
      }
    }
    */

    // need to do something to handle binding scope properly 
    public VCExpr ToVC(VCExpressionGenerator gen, Boogie2VCExprTranslator translator, SortedDictionary<(string op, int size), Function> bvOps, List<Function> newBVFunctions, UniqueNamer namer) {
      Stack<SExpr> todo = new Stack<SExpr>();
      Stack<SExpr> waiting = new Stack<SExpr>();
      Stack<VCExpr> results = new Stack<VCExpr>();
      Stack<VCExprLetBinding> letBindings = new Stack<VCExprLetBinding>();
      HashSet<string> letNames = new HashSet<string>();
      HashSet<string> quantNames = new HashSet<string>();
      Dictionary<string, VCExprVar> boundVars = new Dictionary<string, VCExprVar>();
      todo.Push(this);
      while (todo.Count > 0) {
        SExpr next = todo.Pop();
        if (next.Name == "let") {
          foreach (SExpr def in next[0].Arguments) {
            letNames.Add(def.Name);
          }
        }
        if (next.Name == "exists" || next.Name == "forall") {
          foreach (SExpr def in next[0].Arguments) {
            quantNames.Add(def.Name);
          }
        }
        waiting.Push(next);
        // _ is a special case, used to parameterise functions and types
        if (next.Name != "_" && !quantNames.Contains(next.Name)) {
          foreach (SExpr arg in next.Arguments) {
            if (!(next.Name == "" && arg.Name == "_")) {
              todo.Push(arg);
            }
          }
        }
      }
      while (waiting.Count > 0) {
        SExpr next = waiting.Pop();
        if (next.ArgCount == 0) {
          if (next.Name == "true") {
            results.Push(VCExpressionGenerator.True);
            continue;
          } else if (next.Name == "false") {
            results.Push(VCExpressionGenerator.False);
            continue;
          } else {
            BigNum num;
            BigDec dec;
            if (next.Name.All(Char.IsDigit)) {
              if (BigNum.TryParse(next.Name, out num)) {
                // int
                results.Push(gen.Integer(num));
                continue;
              }
            }
            if (next.Name.All(c => Char.IsDigit(c) || c == '.' || c == 'e')) {
              if (BigDec.TryParse(next.Name, out dec)) {
                // real
                results.Push(gen.Real(dec));
                continue;
              }
            }
            // bad way of handling bitvector literals, will only work for up to 64 bits, but will work for now
            if (next.Name.StartsWith("#b")) {
              int bits = next.Name.Length - 2;
              try {
                num = BigNum.FromULong(Convert.ToUInt64(next.Name.Substring(2), 2));
                results.Push(gen.Bitvector(new BvConst(num, bits)));
                continue;
              } catch {

              }
            }
            // identifier
            VCExprVar boundVar;
            if (boundVars.TryGetValue(next.Name, out boundVar)) {
              results.Push(boundVar);
              continue;
            }
            Object obj;
            if (namer.NameToRef.TryGetValue(next.Name, out obj)) {
              if (obj is VCExprVar) {
                results.Push((VCExprVar)obj);
                continue;
              }
            }

            /*
            bool varFound = false;
            foreach (Variable v in scopeVars) {
              if (v.Name.Equals(next.Name)) {
                results.Push(translator.LookupVariable(v));
                varFound = true;
                break;
              }
            }
            if (varFound) {
              continue;
            } */

            throw new NotImplementedException("unimplemented for conversion to VCExpr: " + next);
          }
        } else {
          if (next.Name == "") {
            if (next[0].Name == "_") {
              SExpr BVOp = next[0];
              if (BVOp.ArgCount > 0) {
                List<VCExpr> BVOpArgs = new List<VCExpr>();
                for (int i = 1; i < next.ArgCount; i++) {
                  BVOpArgs.Add(results.Pop());
                }
                BVOpArgs.Reverse();
                switch (BVOp.Arguments[0].Name) {
                  case "extract":
                    int end = Int32.Parse(BVOp.Arguments[1].Name) + 1;
                    int start = Int32.Parse(BVOp.Arguments[2].Name);
                    int bits = BVOpArgs[0].Type.BvBits;
                    results.Push(gen.BvExtract(BVOpArgs[0], bits, start, end));
                    continue;
                  case "zero_extend":
                  case "sign_extend":
                    int extend = Int32.Parse(BVOp.Arguments[1].Name);
                    int size = BVOpArgs[0].Type.BvBits;
                    string fnName = BVOp.Arguments[0].Name + " " + extend;
                    int outBits = size + extend;

                    Function bvFn;
                    if (bvOps.TryGetValue((fnName, size), out bvFn)) {
                      results.Push(gen.Function(gen.BoogieFunctionOp(bvFn), BVOpArgs));
                    } else {
                      List<Variable> inParams = new List<Variable>();
                      foreach (VCExpr arg in BVOpArgs) {
                        inParams.Add(new Formal(Token.NoToken, new TypedIdent(Token.NoToken, "", arg.Type), true));
                      }
                      Variable result = new Formal(Token.NoToken, new TypedIdent(Token.NoToken, "", Type.GetBvType(outBits)), false);
                      bvFn = new Function(Token.NoToken, "__" + BVOp.Arguments[0].Name + extend + "_" + size, inParams, result);
                      bvFn.Attributes = new QKeyValue(Token.NoToken, "bvbuiltin", new List<Object> { fnName }, null);
                      bvOps.Add((fnName, size), bvFn);
                      newBVFunctions.Add(bvFn);
                      results.Push(gen.Function(gen.BoogieFunctionOp(bvFn), BVOpArgs));
                    }
                    continue;
                  default:
                    // others that might need to be handled:
                    // repeat, rotate_left, rotate_right
                    throw new NotImplementedException("unimplemented for conversion to VCExpr: " + next);
                }
              } else {
                throw new NotImplementedException("unimplemented for conversion to VCExpr: " + next);
              }
            }
            continue;
          }
          // special symbol that indicates function/literal is being parameterised 
          if (next.Name == "_") {
            // bv literal shorthand function
            if (next[0].Name.StartsWith("bv")) {
              BigNum bvValue;
              if (BigNum.TryParse(next.Arguments[0].Name.Substring(2), out bvValue)) {
                int bits;
                if (Int32.TryParse(next.Arguments[1].Name, out bits)) {
                  results.Push(gen.Bitvector(new BvConst(bvValue, bits)));
                  continue;
                }
              }
            }
          }
          if (letNames.Contains(next.Name)) {
            VCExpr e = results.Pop();
            VCExprVar bound = gen.Variable(next.Name, e.Type);
            letBindings.Push(gen.LetBinding(bound, e));
            boundVars.Add(next.Name, bound);
            continue;
          } else if (next.Name == "let") {
            List<VCExprLetBinding> bindings = new List<VCExprLetBinding>();
            for (int i = 0; i < next[0].ArgCount; i++) {
              bindings.Add(letBindings.Pop());
            }
            foreach (VCExprLetBinding binding in bindings) {
              boundVars.Remove(binding.V.Name);
            }
            bindings.Reverse();
            results.Push(gen.Let(bindings, results.Pop()));
            continue;
          } else if (quantNames.Contains(next.Name)) {
            Type boundType;
            // need to handle bv?
            switch(next[0].Name) {
              case "Int":
                boundType = Type.Int;
                break;
              case "Bool":
                boundType = Type.Bool;
                break;
              case "Real":
                boundType = Type.Real;
                break;
              case "_":
              default:
                throw new NotImplementedException("unimplemented for conversion to VCExpr: " + next);
            }
            boundVars.Add(next.Name, gen.Variable(next.Name, boundType));
            continue;
          } else if (next.Name == "exists" || next.Name == "forall") {
            List<VCExprVar> quantVars = new List<VCExprVar>();
            foreach (SExpr def in next[0].Arguments) {
              VCExprVar boundVar;
              if (boundVars.TryGetValue(def.Name, out boundVar)) {
                quantVars.Add(boundVar);
                boundVars.Remove(def.Name);
              }
            }
            VCExpr quantifier;
            if (next.Name == "forall") {
              quantifier = gen.Forall(quantVars, new List<VCTrigger>(), results.Pop());
            } else {
              quantifier = gen.Exists(quantVars, new List<VCTrigger>(), results.Pop());
            }
            results.Push(quantifier);
            continue;
          }
          List<VCExpr> args = new List<VCExpr>();
          for (int i = 0; i < next.ArgCount; i++) {
            args.Add(results.Pop());
          }
          args.Reverse();
          VCExpr combinedArgs;

          if (next.Name.StartsWith("bv")) {
            // bv ops
            int size = args[0].Type.BvBits;
            Function bvFn;
            if (bvOps.TryGetValue((next.Name, size), out bvFn)) {
              combinedArgs = gen.Function(gen.BoogieFunctionOp(bvFn), args);
              results.Push(combinedArgs);
              continue;
            } else {
              List<Variable> inParams = new List<Variable>();
              foreach (VCExpr arg in args) {
                inParams.Add(new Formal(Token.NoToken, new TypedIdent(Token.NoToken, "", arg.Type), true));
              }
              Variable result;
              // can't see a better way to do this at the moment, don't think this is all bv operators though
              // confirmed mathsat + z3 extensions not yet handled: bvredor, bvredand
              switch (next.Name) {
                case "bvadd":
                case "bvsub":
                case "bvmul":
                case "bvudiv":
                case "bvurem":
                case "bvsdiv":
                case "bvsrem":
                case "bvsmod":
                case "bvneg":
                case "bvand":
                case "bvor":
                case "bvnot":
                case "bvxor":
                case "bvnand":
                case "bvnor":
                case "bvxnor":
                case "bvshl":
                case "bvlshr":
                case "bvashr":
                  result = new Formal(Token.NoToken, new TypedIdent(Token.NoToken, "", args[0].Type), false);
                  break;
                case "bvult":
                case "bvule":
                case "bvugt":
                case "bvuge":
                case "bvslt":
                case "bvsle":
                case "bvsgt":
                case "bvsge":
                  result = new Formal(Token.NoToken, new TypedIdent(Token.NoToken, "", Type.Bool), false);
                  break;
                case "bvcomp":
                  result = new Formal(Token.NoToken, new TypedIdent(Token.NoToken, "", Type.GetBvType(1)), false);
                  break;
                case "bv2nat":
                case "bv2int":
                  result = new Formal(Token.NoToken, new TypedIdent(Token.NoToken, "", Type.Int), false);
                  break;
                default:
                  throw new NotImplementedException("unimplemented for conversion to VCExpr: " + next);
              }
              bvFn = new Function(Token.NoToken, "__" + next.Name + size, inParams, result);
              bvFn.Attributes = new QKeyValue(Token.NoToken, "bvbuiltin", new List<Object> { next.Name }, null);
              bvOps.Add((next.Name, size), bvFn);
              newBVFunctions.Add(bvFn);
              combinedArgs = gen.Function(gen.BoogieFunctionOp(bvFn), args);
              results.Push(combinedArgs);
              continue;
            }
          }

          switch (next.Name) {
            case "and":
              combinedArgs = gen.AndSimp(args[0], args[1]);
              for (int i = 2; i < args.Count; i++) {
                combinedArgs = gen.AndSimp(combinedArgs, args[i]);
              }
              results.Push(combinedArgs);
              continue;
            case "or":
              combinedArgs = gen.OrSimp(args[0], args[1]);
              for (int i = 2; i < args.Count; i++) {
                combinedArgs = gen.OrSimp(combinedArgs, args[i]);
              }
              results.Push(combinedArgs);
              continue;
            case "not":
              results.Push(gen.NotSimp(args[0]));
              continue;
            case "=>":
              results.Push(gen.ImpliesSimp(args[0], args[1]));
              continue;
            case "+":
              if (args[0].Type.IsInt) {
                combinedArgs = gen.Function(VCExpressionGenerator.AddIOp, new List<VCExpr> { args[0], args[1] });
                for (int i = 2; i < args.Count; i++) {
                  combinedArgs = gen.Function(VCExpressionGenerator.AddIOp, new List<VCExpr> { combinedArgs, args[i] });
                }
                results.Push(combinedArgs);
                continue;
              } else if (args[0].Type.IsReal) {
                combinedArgs = gen.Function(VCExpressionGenerator.AddROp, new List<VCExpr> { args[0], args[1] });
                for (int i = 2; i < args.Count; i++) {
                  combinedArgs = gen.Function(VCExpressionGenerator.AddROp, new List<VCExpr> { combinedArgs, args[i] });
                }
                results.Push(combinedArgs);
                continue;
              }
              throw new NotImplementedException("unimplemented for conversion to VCExpr: " + next);
            case "-":
              if (args.Count == 1) {
                if (args[0] is VCExprIntLit) {
                  BigNum val = ((VCExprIntLit)args[0]).Val;
                  results.Push(gen.Integer(-val));
                  continue;
                } else if (args[0] is VCExprRealLit) {
                  BigDec val = ((VCExprRealLit)args[0]).Val;
                  results.Push(gen.Real(-val));
                  continue;
                } else {
                  if (args[0].Type.IsInt) {
                    results.Push(gen.Function(VCExpressionGenerator.SubIOp, gen.Integer(BigNum.ZERO), args[0]));
                    continue;
                  } else if (args[0].Type.IsReal) {
                    results.Push(gen.Function(VCExpressionGenerator.SubROp, gen.Real(BigDec.ZERO), args[0]));
                    continue;
                  }
                }
              } else if (args.Count == 2) {
                if (args[0].Type.IsInt) {
                  results.Push(gen.Function(VCExpressionGenerator.SubIOp, args));
                  continue;
                } else if (args[0].Type.IsReal) {
                  results.Push(gen.Function(VCExpressionGenerator.SubROp, args));
                  continue;
                }
              }
              throw new NotImplementedException("unimplemented for conversion to VCExpr: " + next);
            case "*":
              if (args[0].Type.IsInt) {
                combinedArgs = gen.Function(VCExpressionGenerator.MulIOp, new List<VCExpr> { args[0], args[1] });
                for (int i = 2; i < args.Count; i++) {
                  combinedArgs = gen.Function(VCExpressionGenerator.MulIOp, new List<VCExpr> { combinedArgs, args[i] });
                }
                results.Push(combinedArgs);
                continue;
              } else if (args[0].Type.IsReal) {
                combinedArgs = gen.Function(VCExpressionGenerator.MulROp, new List<VCExpr> { args[0], args[1] });
                for (int i = 2; i < args.Count; i++) {
                  combinedArgs = gen.Function(VCExpressionGenerator.MulROp, new List<VCExpr> { combinedArgs, args[i] });
                }
                results.Push(combinedArgs);
                continue;
              }
              throw new NotImplementedException("unimplemented for conversion to VCExpr: " + next);
            case "div":
              results.Push(gen.Function(VCExpressionGenerator.DivIOp, args));
              continue;
            case "/":
              results.Push(gen.Function(VCExpressionGenerator.DivROp, args));
              continue;
            case "mod":
              results.Push(gen.Function(VCExpressionGenerator.ModOp, args));
              continue;
            case "=":
              results.Push(gen.Function(VCExpressionGenerator.EqOp, args));
              continue;
            case ">":
              results.Push(gen.Function(VCExpressionGenerator.GtOp, args));
              continue;
            case "<":
              results.Push(gen.Function(VCExpressionGenerator.LtOp, args));
              continue;
            case "<=":
              results.Push(gen.Function(VCExpressionGenerator.LeOp, args));
              continue;
            case ">=":
              results.Push(gen.Function(VCExpressionGenerator.GeOp, args));
              continue;
            case "ite":
              results.Push(gen.Function(VCExpressionGenerator.IfThenElseOp, args));
              continue;
            case "to_int":
              results.Push(gen.Function(VCExpressionGenerator.ToIntOp, args));
              continue;
            case "to_real":
              results.Push(gen.Function(VCExpressionGenerator.ToRealOp, args));
              continue;
            case "concat":
              results.Push(gen.BvConcat(args[0], args[1]));
              continue;
            default:

              throw new NotImplementedException("unimplemented for conversion to VCExpr: " + next);
          }
        }
      }
      if (results.Count != 1) {
        throw new Exception("something went wrong wiht conversion to VCExpr: " + this);
      }
      return results.Pop();
    }

    /*
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
    */
  }

}