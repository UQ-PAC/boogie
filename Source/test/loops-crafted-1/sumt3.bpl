procedure main() {
  var n: int, i: int, k: int, j: int, l: int;
  assume(n >= 0);
  assume(n <= 20000001);
  i := 0;
  j := 0;
  k := 0;
  l := 0;
  while( l < n ) {
	
	  if((l mod 3) == 0) {
	    i := i + 1;
    } else if((l mod 2) == 0) {
      j := j+1;
    } else {
	    k := k+1;
    }
    l := l+1;
  }
  assert((i+j+k) == l);
  
}

