procedure main() {
  var x: int;
  var y: int;
  
  
  assume(x==1 && y==1);
  while(*) {
    x := x+y;
    y := x;
    
  }
  assert(y>=1);
  
}
