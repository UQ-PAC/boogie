//https://gitlab.com/sosy-lab/benchmarking/sv-benchmarks/-/blob/main/c/bitvector/num_conversion_2.c
function {:bvbuiltin "bvadd"} bv3add(bv3,bv3) returns(bv3);
function {:bvbuiltin "bvand"} bv3and(bv3,bv3) returns(bv3);
function {:bvbuiltin "bvshl"} bv3shl(bv3,bv3) returns(bv3);
function {:bvbuiltin "bvult"} bv3ult(bv3,bv3) returns(bool);

procedure main()
{
  var x: bv3;
  var y: bv3;
  var c: bv3;
  var i: bv3;
  var bit: bv3;

  x := 4bv3;
  y := 0bv3;
  c := 0bv3;

  while (bv3ult(c, 3bv3)) {
    i := bv3shl(1bv3, c);
    bit := bv3and(x, i);
    if (bit != 0bv3) {
      y := bv3add(y, 1bv3);
    }
    c := bv3add(c, 1bv3);
  }
  assert (x == y);
}
