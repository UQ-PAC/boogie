function {:bvbuiltin "bvadd"} bv64add(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvule"} bv64ule(bv64,bv64) returns(bool);
function {:bvbuiltin "bvult"} bv64ult(bv64,bv64) returns(bool);
function {:bvbuiltin "bvslt"} bv64slt(bv64,bv64) returns(bool);
function {:bvbuiltin "bvsub"} bv64sub(bv64,bv64) returns(bv64);

procedure test() {
  var x: bv64;
  var n: bv64;
  var y: bv64;
  assume(bv64ule(0bv64, n));
  x := n;
  y := 0bv64;
  while (bv64ult(0bv64,x)) {
    y := bv64add(y, 1bv64);
    x := bv64sub(x, 1bv64);
  }
  assert (y == n);
}