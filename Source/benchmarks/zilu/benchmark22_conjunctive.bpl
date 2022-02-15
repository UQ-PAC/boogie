procedure main() {
  var x: int;
  var y: int;
  
  assume(x==1 && y==0);
  while(*) {
    x := x+y;
    y := y + 1;
    
  }
  assert(x >= y);
  
}
