procedure main() {
  var x: int;
  var n: int;
  
  assume(x==0 && n>0);
  while (x<n) {
    x := x + 1;
  }
  assert(x==n);
  
}
