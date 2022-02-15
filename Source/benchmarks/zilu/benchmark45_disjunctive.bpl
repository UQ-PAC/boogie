procedure main() {
  var x: int;
  var y: int;
  
  assume(y>0 || x>0);
  while(*) {
    if (x>0) {
      x := x + 1;
    } else {
      y := y + 1;
    }
    
  }
  assert(x>0 || y>0);
  
}
