
function booltobv1(bool) returns (bv1);
axiom booltobv1(true) == 1bv1 && booltobv1(false) == 0bv1;

function bv1tobool(bv1) returns (bool);
axiom bv1tobool(1bv1) == true && bv1tobool(0bv1) == false;

// TODO signed or unsigned div
procedure malloc(size: bv64) returns (addr: bv64, Gamma_addr: SecurityLevel);
ensures (forall i: bv64 :: ((bv64ule(0bv64, i) && bv64ult(i, bv64udiv(size, 4bv64))) ==> old(heap_free[bv64add(addr, i)]) == true)); 
ensures (forall i: bv64 :: ((bv64ule(0bv64, i) && bv64ult(i, bv64udiv(size, 4bv64))) ==> heap_free[bv64add(addr, i)] == false)); 
ensures heap_sizes[addr] == bv64udiv(size, 4bv64);
ensures Gamma_addr == s_TRUE;

procedure free_(addr: bv64) returns ();
ensures (forall i: bv64 :: (bv64ule(0bv64, i) && bv64ult(i, bv64udiv(heap_sizes[addr], 4bv64))) ==> heap_free[bv64add(addr, i)] == true); 


/*****
 * Bitvector functions for bv1
 ****/
// Arithmetic
function {:bvbuiltin "bvadd"} bv1add(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvsub"} bv1sub(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvmul"} bv1mul(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvudiv"} bv1udiv(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvurem"} bv1urem(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvsdiv"} bv1sdiv(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvsrem"} bv1srem(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvsmod"} bv1smod(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvneg"} bv1neg(bv1) returns(bv1);

// Bitwise operations
function {:bvbuiltin "bvand"} bv1and(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvor"} bv1or(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvnot"} bv1not(bv1) returns(bv1);
function {:bvbuiltin "bvxor"} bv1xor(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvnand"} bv1nand(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvnor"} bv1nor(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvxnor"} bv1xnor(bv1,bv1) returns(bv1);

// Bit shifting
function {:bvbuiltin "bvshl"} bv1shl(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvlshr"} bv1lshr(bv1,bv1) returns(bv1);
function {:bvbuiltin "bvashr"} bv1ashr(bv1,bv1) returns(bv1);

// Unsigned comparison
function {:bvbuiltin "bvult"} bv1ult(bv1,bv1) returns(bool);
function {:bvbuiltin "bvule"} bv1ule(bv1,bv1) returns(bool);
function {:bvbuiltin "bvugt"} bv1ugt(bv1,bv1) returns(bool);
function {:bvbuiltin "bvuge"} bv1uge(bv1,bv1) returns(bool);

// Signed comparison
function {:bvbuiltin "bvslt"} bv1slt(bv1,bv1) returns(bool);
function {:bvbuiltin "bvsle"} bv1sle(bv1,bv1) returns(bool);
function {:bvbuiltin "bvsgt"} bv1sgt(bv1,bv1) returns(bool);
function {:bvbuiltin "bvsge"} bv1sge(bv1,bv1) returns(bool);

function {:bvbuiltin "bvcomp"} bv1comp(bv1,bv1) returns(bv1);


/*****
 * Bitvector functions for bv32
 ****/
// Arithmetic
function {:bvbuiltin "bvadd"} bv32add(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvsub"} bv32sub(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvmul"} bv32mul(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvudiv"} bv32udiv(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvurem"} bv32urem(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvsdiv"} bv32sdiv(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvsrem"} bv32srem(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvsmod"} bv32smod(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvneg"} bv32neg(bv32) returns(bv32);

// Bitwise operations
function {:bvbuiltin "bvand"} bv32and(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvor"} bv32or(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvnot"} bv32not(bv32) returns(bv32);
function {:bvbuiltin "bvxor"} bv32xor(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvnand"} bv32nand(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvnor"} bv32nor(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvxnor"} bv32xnor(bv32,bv32) returns(bv32);

// Bit shifting
function {:bvbuiltin "bvshl"} bv32shl(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvlshr"} bv32lshr(bv32,bv32) returns(bv32);
function {:bvbuiltin "bvashr"} bv32ashr(bv32,bv32) returns(bv32);

// Unsigned comparison
function {:bvbuiltin "bvult"} bv32ult(bv32,bv32) returns(bool);
function {:bvbuiltin "bvule"} bv32ule(bv32,bv32) returns(bool);
function {:bvbuiltin "bvugt"} bv32ugt(bv32,bv32) returns(bool);
function {:bvbuiltin "bvuge"} bv32uge(bv32,bv32) returns(bool);

// Signed comparison
function {:bvbuiltin "bvslt"} bv32slt(bv32,bv32) returns(bool);
function {:bvbuiltin "bvsle"} bv32sle(bv32,bv32) returns(bool);
function {:bvbuiltin "bvsgt"} bv32sgt(bv32,bv32) returns(bool);
function {:bvbuiltin "bvsge"} bv32sge(bv32,bv32) returns(bool);


/*****
 * Bitvector functions for bv64
 ****/
// Arithmetic
function {:bvbuiltin "bvadd"} bv64add(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvsub"} bv64sub(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvmul"} bv64mul(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvudiv"} bv64udiv(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvurem"} bv64urem(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvsdiv"} bv64sdiv(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvsrem"} bv64srem(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvsmod"} bv64smod(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvneg"} bv64neg(bv64) returns(bv64);

// Bitwise operations
function {:bvbuiltin "bvand"} bv64and(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvor"} bv64or(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvnot"} bv64not(bv64) returns(bv64);
function {:bvbuiltin "bvxor"} bv64xor(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvnand"} bv64nand(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvnor"} bv64nor(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvxnor"} bv64xnor(bv64,bv64) returns(bv64);

// Bit shifting
function {:bvbuiltin "bvshl"} bv64shl(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvlshr"} bv64lshr(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvashr"} bv64ashr(bv64,bv64) returns(bv64);

// Unsigned comparison
function {:bvbuiltin "bvult"} bv64ult(bv64,bv64) returns(bool);
function {:bvbuiltin "bvule"} bv64ule(bv64,bv64) returns(bool);
function {:bvbuiltin "bvugt"} bv64ugt(bv64,bv64) returns(bool);
function {:bvbuiltin "bvuge"} bv64uge(bv64,bv64) returns(bool);

// Signed comparison
function {:bvbuiltin "bvslt"} bv64slt(bv64,bv64) returns(bool);
function {:bvbuiltin "bvsle"} bv64sle(bv64,bv64) returns(bool);
function {:bvbuiltin "bvsgt"} bv64sgt(bv64,bv64) returns(bool);
function {:bvbuiltin "bvsge"} bv64sge(bv64,bv64) returns(bool);


procedure main() {
var ZF: bv1;
var CF: bv1;
var NF: bv1;
var VF: bv1;
var R0: bv64;
var #33: bv64;
var R3: bv64; 
var R2: bv64;
var #35: bool;
var X1: bv64; 
var R1: bv64;
var R5: bv64;
var R6: bv64; 
var R4: bv64; 
var X0: bv64;
label000004d9:
    X0 := 69632bv64;    // 000001be
    X0 := bv64add(X0, 52bv64);    // 000001c5
    heap[bv64add(X0, 0bv64)] := 0bv64[8:0]; heap[bv64add(X0, 1bv64)] := 0bv64[16:8]; heap[bv64add(X0, 2bv64)] := 0bv64[24:16]; heap[bv64add(X0, 3bv64)] := 0bv64[32:24];     // 000001cc
    goto label000001d1;

label000001d1:
    X0 := 69632bv64;    // 000001d9
    X0 := bv64add(X0, 52bv64);    // 000001e0
    X0 := 0bv32 ++ heap[bv64add(X0, 0bv64)] ++ heap[bv64add(X0, 1bv64)] ++ heap[bv64add(X0, 2bv64)] ++ heap[bv64add(X0, 3bv64)];    // 000001e7
    #33 := bv64add(18446744073709551612bv64, X0);    // 000001ed
    NF := #33[64:63];    // 000001f1
    VF := bv1and(X0[64:63], bv1neg(#33[64:63]));    // 000001f5
    ZF := booltobv1(bv64eq(#33, 0bv64));    // 000001f9
    CF := bv1and(bv1or(bv1or(X0[64:63], bv1neg(#33[64:63])), X0[64:63]), bv1neg(#33[64:63]));    // 000001fd
    #35 := NF != VF || ZF != 0bv1    // 0000020c
    if (#35) { 
      goto label00000204;
    } 
    goto label0000023e;

label00000204:
    X0 := 69632bv64;    // 00000212
    X0 := bv64add(X0, 52bv64);    // 00000219
    X0 := 0bv32 ++ heap[bv64add(X0, 0bv64)] ++ heap[bv64add(X0, 1bv64)] ++ heap[bv64add(X0, 2bv64)] ++ heap[bv64add(X0, 3bv64)];    // 00000220  
    X1 := bv64add(X0, 1bv64);    // 00000227
    X0 := 69632bv64;    // 0000022e
    X0 := bv64add(X0, 52bv64);    // 00000235
    heap[bv64add(X0, 0bv64)] := X1[8:0]; heap[bv64add(X0, 1bv64)] := X1[16:8]; heap[bv64add(X0, 2bv64)] := X1[24:16]; heap[bv64add(X0, 3bv64)] := X1[32:24];     // 0000023c
    goto label000001d1;

label0000023e:  
    X0 := 0bv64;    // 00000243
return;
}