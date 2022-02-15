procedure main() {
  var j: int;
  var k: int;
  var n: int;
  assume((j==0) && (k==n) && (n>0));
  while (j<n && n>0) {
    j := j + 1;
    k := k - 1;
  }
  assert((k == 0));
  
}
