//https://gitlab.com/sosy-lab/benchmarking/sv-benchmarks/-/blob/main/c/bitvector/num_conversion_1.c
function {:bvbuiltin "bvadd"} bv8add(bv8,bv8) returns(bv8);
function {:bvbuiltin "bvand"} bv8and(bv8,bv8) returns(bv8);
function {:bvbuiltin "bvshl"} bv8shl(bv8,bv8) returns(bv8);
function {:bvbuiltin "bvult"} bv8ult(bv8,bv8) returns(bool);

procedure main()
{
  var x: bv8;
  var y: bv8;
  var c: bv8;
  var i: bv8;
  var bit: bv8;

  //x := 37bv8;
  y := 0bv8;
  c := 0bv8;

  while (bv8ult(c, 8bv8)) {
    i := bv8shl(1bv8, c);
    bit := bv8and(x, i);
    if (bit != 0bv8) {
      y := bv8add(y, 1bv8);
    }
    c := bv8add(c, 1bv8);
  }
  assert (x == y);
}
