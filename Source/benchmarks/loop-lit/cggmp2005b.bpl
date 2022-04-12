procedure main() {
  var i: int, j: int, k: int;
  i := 0;
  k := 9;
  j := -100;
  while (i <= 100) {
    i := i + 1;
    while (j < 20) {
      j := i + j;
    }
    k := 4;
    while (k <= 3) {
      k := k + 1;
    }
  }
  assert(k == 4);
}
