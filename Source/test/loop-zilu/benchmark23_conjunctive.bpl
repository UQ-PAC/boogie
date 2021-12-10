procedure main() {
  var i: int;
  var j: int;
  
  assume(i==0 && j==0);
  while (i<100) {
    j := j + 2;
    i := i + 1;
  }
  assert(j==200);
  
}
