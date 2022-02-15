procedure main() {
  var i: int;
  var k: int;
  
  assume(0 <= k && k <= 1 && i == 1);
  while(*) {
    i := i + 1;
    k := k - 1;
    
  }
  assert(1 <= i + k && i + k <= 2 && i >= 1);
  
}
