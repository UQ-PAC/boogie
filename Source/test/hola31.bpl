procedure hola31() {
  var i: int;
  var j: int;
  var k: int;
  var n: int;
  var m: int;
  var block: int;
  block := 0;

  assume(m+1 < n);
  i := 0;

  while (block != -1) {
    if (block == 0) {
      if (i < n) {
        j := i;
        block := 1;
      } else {
        block := -1;
      }
    } else if (block == 1) {
      if (j < m) {
        if (*) {
          assert (j >= 0);
          j := j + 1;
          k := 0;
          block := 2;
        } else {
          assert (n + j + 5 > i);
          j := j + 2;
        }
      } else {
        i := i + 4;
        block := 0;
      }
    } else if (block == 2) {
      if (k < j) {
        k := k + 1;
      } else {
        block := 1;
      }
    }
  }
}


