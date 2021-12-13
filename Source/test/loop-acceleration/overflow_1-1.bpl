procedure main() {
  var x: int;
 x := 10;

  while (x >= 10) {
    x := x + 2;
  }

  assert((x mod 2) == 0);
}
