procedure main() {
  var k: int;
  var j: int;
  var n: int;
  
  assume(n>=1 && k>=n && j==0);
  while (j<=n-1) {
    j := j + 1;
    k := k - 1;
  }
  assert(k>=0);
  
}
