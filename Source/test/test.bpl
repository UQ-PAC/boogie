procedure test() {
  var x: int, n: int;
  assume (n >= 0);
  while (x < n) {
      x := x + 1;
  }
  assert (x == n);
}