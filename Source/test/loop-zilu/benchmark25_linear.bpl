procedure main() {
  var x: int;
  assume(x<0);
  while (x<10) {
    x := x+1;
  }
  assert(x==10);
  
}
