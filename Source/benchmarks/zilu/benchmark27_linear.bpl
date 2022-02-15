procedure main() {
  var i: int;
  var j: int;
  var k: int;
  assume(i<j && k>i-j);
  while (i<j) {
    k := k+1;
    i := i+1;
  }
  assert(k > 0);
  
}
