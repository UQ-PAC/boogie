procedure main() {
  var x: int;
 x := 0;

  while (x < 268435455) {
    x := x + 2;
  }

  assert(x mod 2 != 0);
}
