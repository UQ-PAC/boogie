procedure main() {
  var x: int;
  var N: int;
   x := 0;
  assume (N >= 0 && N <= 65535);

  while (x < N) {
    x := x + 2;
  }

  assert((x mod 2) == 0);
}
