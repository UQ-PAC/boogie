//https://gitlab.com/sosy-lab/benchmarking/sv-benchmarks/-/blob/main/c/bitvector/parity.c
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

procedure main()
{
    var v: bv32;
    var v1: bv32;
    var v2: bv32;
    var parity1: bv8;
    var parity2: bv8;

    /* naive parity */
    v1 := v;
    parity1 := 0bv8;
    while (v1 != 0bv32) {
        if (parity1 == 0bv8) {
            parity1 := 1bv8;
        } else {
            parity1 := 0bv8;
        }
        v1 := bv32and(v1, bv32sub(v1, 1bv32));
    }

    /* smart parity */
    v2 := v;
    parity2 := 0bv8;
    v2 := bv32xor(v2, bv32lshr(v2, 1bv32));
    v2 := bv32xor(v2, bv32lshr(v2, 2bv32));
    v2 := bv32mul(bv32and(v2, 286331153bv32), 286331153bv32); /* 286331153U == 0x11111111U */
    if (bv32and(bv32lshr(v2,28bv32), 1bv32) == 0bv32) {
        parity2 := 0bv8;
    } else {
        parity2 := 1bv8;
    }

    assert(parity1 == parity2);
}
