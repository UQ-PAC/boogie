procedure main() {
  var i: int;
  var n: int;
  var sum: int;
  
  assume(i==0 && n>=0 && n<=100 && sum==0);
  while (i<n) {
    sum := sum + i;
    i := i + 1;
  }
  assert(sum>=0);
  
}
