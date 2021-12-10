procedure main() {
  var i: int;
  var k: int;
  var n: int;
  assume((i==0) && (k==0) && (n>0));
  while (i < n) {
    i := i + 1;
    k := k + 1;
  }
  assert((i == k) && (k == n));
  
}
