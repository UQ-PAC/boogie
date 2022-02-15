procedure main() {
  var x: int;
  var y: int;
  
  assume(x == y && y == 0);
  while(*) {
    x := x + 1;
    y := y + 1;
    
  }
  assert(x == y && x >= 0);
  
}
