procedure main() {
  var i: int;
  var l: int;
  var n: int;
  var k: int;

  assume(l>0);
  assume(l < 100000);
  assume(n < 100000);
  k := 1;
  while (k < n) {
    i := l;
    while (i < n) {
      assert (1 <= i);
      i := i + 1;
    }
    if (*) {
      l := l + 1;
    }
    k := k + 1;
  }
}