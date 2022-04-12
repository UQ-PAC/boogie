procedure main() {
  var nlen: int;
  var i: int;
  var j: int;
  
  i := 0;
  while (i < nlen) {
    j := 0;
    while (j<8) {
      assert(0 <= nlen-1-i);
      assert(nlen-1-i < nlen);
      j := j + 1;
    }
    i := i + 1;
  }
}  
