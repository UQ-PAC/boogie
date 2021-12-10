procedure main() {
  var i: int;
  
  assume(i>=0 && i<=200);
  while (i>0) {
    i := i - 1;
  }
  assert(i>=0);
  
}
