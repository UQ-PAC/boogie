procedure main() {
  var x: int;
  var y: int;
  
  assume(x*y>=0);
  while(*) {
    if(x==0) {
      if (y>0) {
        x := x + 1;
      } else {
        x := x - 1;
      }
    }
    if(x>0) {
      y := y + 1;
    } else {
      x := x - 1;
    }
    
  }
  assert(x*y>=0);
  
}
