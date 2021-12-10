procedure main() {
  var x: int;
  var y: int;
  var u: bool;
  
  assume(x==1 && y==1);
  while(u) {
    x := x+y;
    y := x;
    havoc u;
  }
  assert(y>=1);
  
}
