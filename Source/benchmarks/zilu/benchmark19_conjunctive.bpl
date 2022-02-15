procedure main() {
  var j: int;
  var k: int;
  var n: int;
  assume((j==n) && (k==n) && (n>0));
  while (j>0 && n>0) {
    j := j - 1;
    k := k - 1;
  }
  assert((k == 0));
  
}
