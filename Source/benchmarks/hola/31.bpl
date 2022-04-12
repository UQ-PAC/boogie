procedure hola31() {
  var i: int;
  var j: int;
  var k: int;
  var n: int;
  var m: int;

  assume(m+1 < n);
  i := 0;

  while (i < n) {
    j := i;
    while (j < m) {
      if (*) {
        assert (j >= 0);
        j := j + 1;
        k := 0;
        while (k < j) {
          k := k + 1;
        }
      } else {
        assert (n + j + 5 > i);
        j := j + 2;
      }
    }
    i := i + 4;
  }
}


