procedure main() {
  var x: int;
 x := 268435440;

  while (x > 0) {
    x := x - 2;
  }

  assert((x mod 2) == 0);
}
