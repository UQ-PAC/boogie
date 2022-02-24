function {:bvbuiltin "bvadd"} bv64add(bv64,bv64) returns(bv64);
function {:bvbuiltin "bvult"} bv64ult(bv64,bv64) returns(bool);
function {:bvbuiltin "bvslt"} bv64slt(bv64,bv64) returns(bool);
function {:bvbuiltin "bvsub"} bv64sub(bv64,bv64) returns(bv64);


procedure test() {
  var x: bv64;
  var n: bv64;
  var y: bv64;
  x := 0bv64;

  assume(bv64ult(0bv64, n));
  y := n;
  while (bv64ult(0bv64, y)) {
    y := bv64sub(y, 1bv64);
    x := bv64add(x, 1bv64);
  }
  assert (x == n);
}