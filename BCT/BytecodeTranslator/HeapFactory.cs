﻿//-----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All Rights Reserved.
//
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Cci;
using Microsoft.Cci.MetadataReader;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.ILToCodeModel;

using Bpl = Microsoft.Boogie;

namespace BytecodeTranslator {

  /// <summary>
  /// Implementations of this interface determine how the heap is represented in
  /// the translated Boogie program.
  /// </summary>
  public interface IHeap {

    /// <summary>
    /// Creates a fresh BPL variable to represent <paramref name="field"/>, deciding
    /// on its type based on the heap representation.
    /// </summary>
    Bpl.Variable CreateFieldVariable(IFieldReference field);

    /// <summary>
    /// Returns the (typed) BPL expression that corresponds to the value of the field
    /// <paramref name="f"/> belonging to the object <paramref name="o"/> (when
    /// <paramref name="o"/> is non-null, otherwise the value of the static field.
    /// </summary>
    /// <param name="o">The expression that represents the object to be dereferenced.
    /// Null if <paramref name="f"/> is a static field.
    /// </param>
    /// <param name="f">The field that is used to dereference the object <paramref name="o"/>, when
    /// it is not null. Otherwise the static field whose value should be read.
    /// </param>
    Bpl.Expr ReadHeap(Bpl.Expr/*?*/ o, Bpl.IdentifierExpr f);

    /// <summary>
    /// Returns the BPL command that corresponds to assigning the value <paramref name="value"/>
    /// to the field <paramref name="f"/> of the object <paramref name="o"/> (when
    /// <paramref name="o"/> is non-null, otherwise it is an assignment to the static
    /// field.
    /// </summary>
    Bpl.Cmd WriteHeap(Bpl.IToken tok, Bpl.Expr/*?*/ o, Bpl.IdentifierExpr f, Bpl.Expr value);

  }

  public abstract class HeapFactory {

    /// <summary>
    /// Returns an object that determines the heap representation.
    /// </summary>
    /// <param name="sink">
    /// The heap might need to generate declarations so it needs access to the Sink.
    /// </param>
    public abstract IHeap MakeHeap(Sink sink);
  }

}