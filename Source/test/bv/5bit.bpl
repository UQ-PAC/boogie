function {:bvbuiltin "bvadd"} bv5add(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvsub"} bv5sub(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvmul"} bv5mul(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvudiv"} bv5udiv(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvurem"} bv5urem(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvsdiv"} bv5sdiv(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvsrem"} bv5srem(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvsmod"} bv5smod(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvneg"} bv5neg(bv5) returns(bv5);
function {:bvbuiltin "bvand"} bv5and(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvor"} bv5or(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvnot"} bv5not(bv5) returns(bv5);
function {:bvbuiltin "bvxor"} bv5xor(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvnand"} bv5nand(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvnor"} bv5nor(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvxnor"} bv5xnor(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvshl"} bv5shl(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvlshr"} bv5lshr(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvashr"} bv5ashr(bv5,bv5) returns(bv5);
function {:bvbuiltin "bvult"} bv5ult(bv5,bv5) returns(bool);
function {:bvbuiltin "bvule"} bv5ule(bv5,bv5) returns(bool);
function {:bvbuiltin "bvugt"} bv5ugt(bv5,bv5) returns(bool);
function {:bvbuiltin "bvuge"} bv5uge(bv5,bv5) returns(bool);
function {:bvbuiltin "bvslt"} bv5slt(bv5,bv5) returns(bool);
function {:bvbuiltin "bvsle"} bv5sle(bv5,bv5) returns(bool);
function {:bvbuiltin "bvsgt"} bv5sgt(bv5,bv5) returns(bool);
function {:bvbuiltin "bvsge"} bv5sge(bv5,bv5) returns(bool);


procedure test() {
  var x: bv5;
  var n: bv5;
  var y: bv5;
  assume(bv5sle(0bv5, n));
  x := n;
  y := 0bv5;
  while (bv5slt(0bv5,x)) {
    y := bv5add(y, 1bv5);
    x := bv5sub(x, 1bv5);
  }
  assert (y == n);
}