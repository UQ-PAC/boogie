procedure main() {
  var x: int;
  var y: int;
  assume(x<y);
  while (x<y) {
    x := x+100;
  }
  assert(x >= y && x <= y + 99);
  
}
