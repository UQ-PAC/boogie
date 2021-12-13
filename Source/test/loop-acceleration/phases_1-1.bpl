procedure main() {
  var x: int;
 x := 0;

  while (x < 268435455) {
    if (x < 65520) {
      x := x + 1;
    } else {
      x := x + 2;
    }
  }

  assert((x mod 2) == 0);
}
