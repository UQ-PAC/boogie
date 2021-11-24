procedure test() {
  var x: int, n: int, y: int;
  var q: int;
  assume (x == 0);
  assume (y == 1);
  assume (n >= 0);
  while (x != n) {
      x := x + 1;
      call y := increment(y);
  }
  assert (y == n + 1);
}

procedure increment(a: int) returns (b: int);
  ensures b == a + 1;