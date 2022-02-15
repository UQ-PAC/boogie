procedure main() {
  var x: int;
  var y: int;
  var n: int;
  
  assume(x>=0 && x<=y && y<n);
  while (x<n) {
    x := x + 1;
    if (x>y) {
      y := y + 1;
    }
  }
  assert(y==n);
  
}
