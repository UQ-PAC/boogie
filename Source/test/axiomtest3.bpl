const n: int;
axiom (n >= 0);

procedure test() {
  var x: int, y: int;
  var q: int;
  x := -1;
  x := x + 1;
  y := 0;
  while (x != n) {
      x := x + 1;
      y := y + 1;
  }
  assert (y >= n);
}