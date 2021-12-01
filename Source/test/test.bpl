procedure test() {
  var x: int, n: int, y: int;
  var q: int;
  assume (x == 0);
  assume (y == 0);
  assume (n >= 0);
  while (x != n) {
      x := x + 1;
      //y := y + 1;
      call y := increment(y);
  }
  assert (y == n);
}

procedure increment(a: int) returns (b: int);
  ensures b == a + 1;