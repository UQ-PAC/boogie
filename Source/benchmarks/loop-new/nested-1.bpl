procedure main() {
  var n: int;
  var m: int;
  var k: int;
  var i: int;
  var j: int;
  k := 0;
  assume(10 <= n && n <= 10000);
  assume(10 <= m && m <= 10000);
  i := 0;
  while (i < n) {
    j := 0;
    while (j < m) {
      k := k + 1;
      j := j + 1;
    }
    i := i + 1;
  }
  assert(k >= 100);
}
