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

function {:bvbuiltin "bvcomp"} bv32comp(bv32,bv32) returns(bv1);
procedure fib21()
{

  var i: bv32;
  var j: bv32;
  var k: bv32;
  var c1: bv32;
  var c2: bv32;
  var n: bv32;
  var v: bv32;

  assume(bv32ugt(n, 0bv32) && bv32ult(n, 10bv32));
  assume(k == 0bv32);
  assume(i == 0bv32);
  assume(c1 == 4000bv32);
  assume(c2 == 2000bv32);

  while(bv32ult(i, n)){
    i := bv32add(i, 1bv32);
    
    
    if(*){
      v := 0bv32;		
    }
    else{
      v := 1bv32;
    }

    if(v == 0bv32){
      k := bv32add(k, c1);
    }
    else{
      k := bv32add(k, c2);
    }
  }

  assert(bv32ugt(k, n));	
}