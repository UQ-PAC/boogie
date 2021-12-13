procedure main() {
  var x: int;
 assume (x >= 0);

  while (x < 268435455) {
    x := x + 1;
  }

  assert(x >= 268435455);
}
