procedure test() {
  var x: int, y: int, n: int;
  assume (n >= 0);
  x := 0;
  if (n >= 2) {
    havoc x, y;
  }
  y := 0;
  while (x != n) {
      x := x + 1;
      y := y + 1;
  }
  assert (y == n);
}