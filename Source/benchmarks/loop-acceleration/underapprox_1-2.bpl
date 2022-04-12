procedure main() {
  var x: int;
    var y: int;
 x := 0;
 y := 1;

  while (x < 6) {
    x := x + 1;
    y := y * 2;
  }

  assert(y mod 3 != 0);
}
