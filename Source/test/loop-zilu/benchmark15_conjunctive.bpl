procedure main() {
  var low: int;
  var mid: int;
  var high: int;
  assume(low == 0 && mid >= 1 && high == 2*mid);
  while (mid > 0) {
    low := low + 1;
    high := high - 1;
    mid := mid - 1;
  }
  assert(low == high);
  
}
