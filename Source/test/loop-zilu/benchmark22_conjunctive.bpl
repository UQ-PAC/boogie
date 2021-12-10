procedure main() {
  var x: int;
  var y: int;
  var u: bool;
  assume(x==1 && y==0);
  while(u) {
    x := x+y;
    y := y + 1;
    havoc u;
  }
  assert(x >= y);
  
}
