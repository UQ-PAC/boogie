procedure test() {
  var x: int, n: int;
  assume (x < n);
  while (x < n) {
      x := x + 1;
  }
  assert (x == n);
}