procedure main() {
  var x: int;
 x := 268435441;

  while (x > 1) {
    x := x - 2;
  }

  assert((x mod 2) == 0);
}
