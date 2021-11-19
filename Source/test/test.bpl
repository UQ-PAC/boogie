procedure test() {
  var x: int, n: int, y: int;
  assume (x == 0);
  assume (y == 0);
  assume (n >= 0);
  while (x != n) {
      x := x + 1;
      y := y + 1;
  }
  assert (y == n);
}