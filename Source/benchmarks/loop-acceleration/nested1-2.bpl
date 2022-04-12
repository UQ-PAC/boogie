procedure main () {
  var x: int;
  var y: int;
  while (x < 268435455) {
    y := 0;
    while (y < 10) {
      y := y + 1;
    }
    x := x + 1;
  }
  assert (x mod 2 == 0)
}