procedure main() {
  var x: int;
    var y: int;
 x := 1;

  assume(y > 0);

  while (x < y) {
    if (x < (y div x)) {
      x := x * x;

    } else {
      x := x + 1;
    }
  }

  assert(x == y);
}
