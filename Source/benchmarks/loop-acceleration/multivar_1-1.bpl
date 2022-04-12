procedure main() {
  var x: int;
    var y: int;
 assume (x >= 0);
 y := x;

  while (x < 1024) {
    x := x + 1;
    y := y + 1;
  }

  assert(x == y);
}
