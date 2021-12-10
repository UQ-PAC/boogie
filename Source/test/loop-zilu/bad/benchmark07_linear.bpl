procedure main() {
  var i: int;
  var n: int;
  var k: int;
  
  var flag: bool;
  i := 0;
  assume(n>0 && n<10);
  while (i<n) {
    i := i + 1;
    if(flag) {
      k := k + 4000;
    } else {
      k := k + 2000;
    }
  }
  assert(k>n);
  
}
