procedure main() {
  var n: int;
  var sum: int;
  var i: int;
  
  assume(n>=0 && sum==0 && i==0);
  while (i<n) {
    sum := sum+i;
    i := i + 1;
  }
  assert(sum>=0);
  
}
