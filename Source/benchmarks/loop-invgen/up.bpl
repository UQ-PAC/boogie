procedure main() {
  var n: int;
  var k: int;
  var i: int;
  var j: int;
  k := 0;
  i := 0;
  while(i < n) {
    i := i + 1;
    k := k + 1;
  }
  j := 0;
  while( j < n ) {
    assert(k > 0);
    j := j + 1;
    k := k - 1;
  }
}
