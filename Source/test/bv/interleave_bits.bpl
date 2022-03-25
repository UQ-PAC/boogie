//https://gitlab.com/sosy-lab/benchmarking/sv-benchmarks/-/blob/main/c/bitvector/interleave_bits.c

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

function {:bvbuiltin "zero_extend 16"} zero_extend16_16(bv16) returns(bv32);

procedure main()
{
    /* Interleave bits of x and y, so that all of the */
    /* bits of x are in the even positions and y in the odd; */
    var x: bv16;
    var y: bv16;

    var xx: bv32;
    var yy: bv32;
    var zz: bv32;

    var z: bv32;
    var i: bv32;

    z := 0bv32; /* z gets the resulting Morton Number. */
    i := 0bv32;

    while (bv32ult(i, 16bv32)) {
      z := bv32or(z, bv32or(bv32shl(bv32and(zero_extend16_16(x), bv32shl(1bv32, i)), i), bv32shl(bv32and(zero_extend16_16(y), bv32shl(1bv32, i)), bv32add(i, 1bv32))));
      i := bv32add(i, 1bv32);
    }

    xx := zero_extend16_16(x);
    yy := zero_extend16_16(y);

    xx := bv32and(bv32or(xx, bv32shl(xx, 8bv32)), 16711935bv32); /* 0x00FF00FF */
    xx := bv32and(bv32or(xx, bv32shl(xx, 4bv32)), 252645135bv32); /* 0x0F0F0F0F */
    xx := bv32and(bv32or(xx, bv32shl(xx, 2bv32)), 858993459bv32); /* 0x33333333 */
    xx := bv32and(bv32or(xx, bv32shl(xx, 1bv32)), 1431655765bv32); /* 0x55555555 */

    yy := bv32and(bv32or(yy, bv32shl(yy, 8bv32)), 16711935bv32); /* 0x00FF00FF */
    yy := bv32and(bv32or(yy, bv32shl(yy, 4bv32)), 252645135bv32); /* 0x0F0F0F0F */
    yy := bv32and(bv32or(yy, bv32shl(yy, 2bv32)), 858993459bv32); /* 0x33333333 */
    yy := bv32and(bv32or(yy, bv32shl(yy, 1bv32)), 1431655765bv32); /* 0x55555555 */
    
    zz := bv32or(xx, bv32shl(yy, 1bv32));

    assert(z == zz);
}
