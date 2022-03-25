//https://gitlab.com/sosy-lab/benchmarking/sv-benchmarks/-/blob/main/c/bitvector-loops/overflow_1-2.c
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
procedure main() {
  var x: bv64;
  x := 10bv64;

  while (bv64uge(x, 10bv64)) {
    x := bv64add(x, 2bv64);
  }
  assert(bv64urem(x, 2bv64) == 0bv64);
}
