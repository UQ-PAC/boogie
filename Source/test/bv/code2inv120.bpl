function {:bvbuiltin "bvule"} bv32ule(bv32,bv32) returns(bool);
function {:bvbuiltin "bvadd"} bv32add(bv32,bv32) returns(bv32);
procedure main() {
  // variable declarations
  var i: bv32;
  var sn: bv32;
  // pre-conditions
  sn := 0bv32;
  i := 1bv32;
  // loop body
  while (bv32ule(i, 8bv32)) {
    i  :=  bv32add(i, 1bv32);
    sn  :=  bv32add(sn, 1bv32);
  }
  // post-condition
  assert((sn != 8bv32) ==> (sn == 0bv32));
}
