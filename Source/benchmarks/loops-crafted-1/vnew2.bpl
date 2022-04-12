procedure main() {
  var n: int, i: int, k: int, j: int;
  var SIZE: int;
  SIZE := 20000001;
  assume (n >= 0);
  assume(n <= SIZE);
  i := 0;
  j := 0;
  k := 0;
  while( i < n ) {
    i := i + 3;
    j := j+3;
    k := k+3;
  }
  if(n>0) {
    assert( i==j && j==k && (i mod (SIZE+2)) != 0 );
  }

}

