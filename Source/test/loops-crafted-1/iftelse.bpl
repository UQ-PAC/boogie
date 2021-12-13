procedure main() {
  var n: int, i: int, k: int, j: int;
  assume(n >= 0);
  assume(n <= 20000001);
  i := 0;
  j := 0;
  k := 0;
  while( i < n ) {
    i := i + 3;
    if((i mod 2) != 0) {
      j := j+3;
    }else {
      k := k+3;
    }
    if(n>0) {
      assert( ((i div 2)<=j) );
    }
  }
}

