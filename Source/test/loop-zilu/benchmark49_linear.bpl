procedure main() {
  var i: int;
  var j: int;
  var r: int;
  assume(r > i + j);
  while (i > 0) {
    i := i - 1;
    j := j + 1;
  }
  assert(r > i + j);
  
}
