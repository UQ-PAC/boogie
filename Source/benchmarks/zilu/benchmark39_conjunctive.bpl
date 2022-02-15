procedure main() {
  var x: int;
  var y: int;
  assume(x == 4*y && x >= 0);
  while (x > 0) {
    x := x - 4;
    y := y - 1;
  }
  assert(y>=0);
  
}
