procedure main() {
  var i: int;
  var j: int;
  var n: int;
  var k: int;

  assume(n < 100000);
  assume(k == n);

  i := 0;
  while (i < n) {
    j := 2 * i;
    while (j < n) {
      if(*) {
        k := j;
        while (k < n) {
          assert(k>=2*i);
          k := k + 1;
        }
      }
      else {
        assert( k >= n );
        assert( k <= n );
      }
      j := j + 1;
    }
    i := i + 1;
  }
}
