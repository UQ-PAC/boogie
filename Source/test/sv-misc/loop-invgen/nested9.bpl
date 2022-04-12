procedure main() {
  var i: int;
  var k: int;
  var j: int;
  var n: int;
  var l: int;
  var m: int;
  assume(-1000000 < n && n < 1000000);
  assume(-1000000 < m && m < 1000000);
  assume(-1000000 < l && l < 1000000);
  assume(3*n<=m+l);
  i := 0;
  while(i<n) {
    j := 2*i;
    while (j<3*i) {
      k := i;
      while (k < j) {
        assert( k-i <= 2*n );
        k := k + 1;
      }
      j := j + 1; 
    }
    i := i + 1;
  }
}
