procedure main() {
  var i: int;
  var k: int;
  var n: int;
  
  assume(i==0 && k==n && n>=0);
  while (i<n) {
    k := k - 1;
    i := i + 2;
  }
  assert(2*k>=n-1);
  
}
