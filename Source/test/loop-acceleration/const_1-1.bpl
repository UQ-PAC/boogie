procedure main() {
  var x: int;
    var y: int;
 x := 1;
 y := 0;

  while (y < 1024) {
    x := 0;
    y := y + 1;
  }

  assert(x == 0);
}
