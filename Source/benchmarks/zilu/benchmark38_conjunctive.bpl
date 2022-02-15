procedure main() {
  var x: int;
  var y: int;
  
  assume(x == y && y == 0);
  while(*) {
    x := x + 4;
    y := y + 1;
    
  }
  assert(x == 4*y && x >= 0);
  
}
