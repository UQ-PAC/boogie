procedure main() {
  var n0: int;
  var n1: int;
  var i0: int;
  var k: int;
  var i1: int;
  var j1: int;
  i0 := 0;
  k := 0;

  //assume(-1000000 <= n0 && n0 < 1000000);
  //assume(-1000000 <= n1 && n1 < 1000000);

  while( i0 < n0 ) {
    i0 := i0 + 1;
    k := k + 1;
  }

  i1 := 0;
  while( i1 < n1 ) {
    i1 := i1 + 1;
    k := k + 1;
  }

  j1 := 0;
  while( j1 < n0 + n1 ) {
    assert(k > 0);
    j1 := j1 + 1;
    k := k - 1;
  }
}
