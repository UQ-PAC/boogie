procedure main() {
  var x: int;
  
  assume(x>=0);
  while (x<100 && x>=0) {
    x := x + 1;
  }
  assert(x>=100);
  
}
