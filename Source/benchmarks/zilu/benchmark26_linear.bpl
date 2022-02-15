procedure main() {
  var x: int;
  var y: int;
  assume(x<y);
  while (x<y) {
    x := x+1;
  }
  assert(x==y);
  
}
