function {:bvbuiltin "bvadd"} bv4add(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvsub"} bv4sub(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvmul"} bv4mul(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvudiv"} bv4udiv(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvurem"} bv4urem(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvsdiv"} bv4sdiv(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvsrem"} bv4srem(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvsmod"} bv4smod(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvneg"} bv4neg(bv4) returns(bv4);
function {:bvbuiltin "bvand"} bv4and(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvor"} bv4or(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvnot"} bv4not(bv4) returns(bv4);
function {:bvbuiltin "bvxor"} bv4xor(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvnand"} bv4nand(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvnor"} bv4nor(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvxnor"} bv4xnor(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvshl"} bv4shl(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvlshr"} bv4lshr(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvashr"} bv4ashr(bv4,bv4) returns(bv4);
function {:bvbuiltin "bvult"} bv4ult(bv4,bv4) returns(bool);
function {:bvbuiltin "bvule"} bv4ule(bv4,bv4) returns(bool);
function {:bvbuiltin "bvugt"} bv4ugt(bv4,bv4) returns(bool);
function {:bvbuiltin "bvuge"} bv4uge(bv4,bv4) returns(bool);
function {:bvbuiltin "bvslt"} bv4slt(bv4,bv4) returns(bool);
function {:bvbuiltin "bvsle"} bv4sle(bv4,bv4) returns(bool);
function {:bvbuiltin "bvsgt"} bv4sgt(bv4,bv4) returns(bool);
function {:bvbuiltin "bvsge"} bv4sge(bv4,bv4) returns(bool);


procedure test() {
  var x: bv4;
  var n: bv4;
  var y: bv4;
  assume(bv4sle(0bv4, n));
  x := n;
  y := 0bv4;
  while (bv4slt(0bv4,x)) {
    y := bv4add(y, 1bv4);
    x := bv4sub(x, 1bv4);
  }
  assert (y == n);
}