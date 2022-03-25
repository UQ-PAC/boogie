
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

function {:bvbuiltin "bvcomp"} bv64comp(bv64,bv64) returns(bv1);

procedure main()    {
var ZF: bv1; 
var CF: bv1;
var NF: bv1;
var VF: bv1;
var R0: bv64;
var #33: bv64;
var #38: bv64;
var #44: bv64;
var R3: bv64;
var R2: bv64;
var #42: bv64;
var #41: bv64; 
var #36: bv1; 
var #34: bv64; 
var R1: bv64; 
var R5: bv64; 
var R6: bv64; 
var #39: bv64; 
var R4: bv64;
var sp8: bv32;
var sp12: bv32;

label00000338:
    R0 := 0bv64;    // 00000118
    R0 := bv64or(bv64and(R0, 18446744069414584320bv64), 1bv64);    // 0000011a
    sp8 := R0[32:0];
    goto label00000121;

label00000121:
    #33 := 0bv32 ++ sp8; // 00000126
    R0 := 0bv64;    // 00000128
    R0 := bv64or(bv64and(R0, 18446744069414584320bv64), #33);    // 0000012a
    #34 := bv64add(18446744073709551606bv64, R0[32:0] ++ 0bv32);    // 0000012e
    NF := #34[64:63];    // 00000130
    VF := bv1and(R0[32:31], bv1neg(#34[64:63]));    // 00000132
    ZF := bv64comp(#34, 0bv64);    // 00000134
    CF := bv1or(bv1and(bv1or(R0[32:31], R0[32:31]), bv1neg(#34[64:63])), bv1neg(#34[64:63]));    // 00000136
    #36 := bv1xor(bv1or(bv1xor(NF, VF), ZF), 0bv1);  // 0000013f
    if (#36 == 1bv1) { goto label00000139; } goto label0000016e;

label00000139:
    R1 := 0bv64;    // 00000142
    R1 := bv64or(bv64and(R1, 18446744069414584320bv64), 10bv64);    // 00000144
    #38 := 0bv32 ++ sp8;    // 00000148
    R0 := 0bv64;    // 0000014a
    R0 := bv64or(bv64and(R0, 18446744069414584320bv64), #38);    // 0000014c
    #39 := bv64sub(R1[32:0] ++ 0bv32, R0[32:0] ++ 0bv32);    // 00000150
    R0 := 0bv64;    // 00000152
    R0 := bv64or(bv64and(R0, 18446744069414584320bv64), #39);    // 00000154
    sp12 := R0[32:0];    // 00000158
    #41 := 0bv32 ++ sp8;    // 0000015c
    R0 := 0bv64;    // 0000015e
    R0 := bv64or(bv64and(R0, 18446744069414584320bv64), #41);    // 00000160
    #42 := bv64add(R0[32:0] ++ 0bv32, 1bv64);    // 00000164
    R0 := 0bv64;    // 00000166
    R0 := bv64or(bv64and(R0, 18446744069414584320bv64), #42);    // 00000168
    sp8 := R0[32:0];  // 0000016c
    goto label00000121;

label0000016e:
    #44 := 0bv32 ++ sp12;  // 00000170
    R0 := 0bv64;    // 00000172
    R0 := bv64or(bv64and(R0, 18446744069414584320bv64), #44);    // 00000174
    assert (bv32sge(R0[32:0], 0bv32));
return;
}