procedure main() {
  var x: int;
  var y: int;
  assume(x == y && x >= 0);
  while (x > 0) {
    x := x - 1;
    y := y - 1;
  }
  assert(y>=0);
  
}
