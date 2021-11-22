procedure test() {
  var x: int, n: int, y: int;
  var q: int;
  q := 2;
  assume (x == 0);
  assume (y == 1);
  assume (n >= 0);
  while (x != n) {
      call x := increment(y);
      call y := increment(y);
  }
  assert (q == 2);
  assert (y == n + 1);
}

procedure increment(a: int) returns (b: int);
  ensures b == a + 1;