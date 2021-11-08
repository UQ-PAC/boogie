procedure test() {
  var x: int, y: int, n: int;
  assume (n >= 0);
  x := 0;
  y := 0;
  while (x != n) {
    if (x == 1) {
      x := x + 1;
      y := y + 1;
    }
  }
  assert (y == n);
}