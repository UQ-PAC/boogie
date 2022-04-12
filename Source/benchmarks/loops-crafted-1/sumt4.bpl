procedure main() {
  var v4: int, i: int, k: int, j: int, l: int, n: int;
  v4 := 0;
  i := 0;
  k := 0;
  j := 0;
  l := 0;
  assume(n >= 0);
  assume(n <= 20000001);
  while( l < n ) {
	
	  if((l mod 4) == 0) {
	    v4 := v4 + 1;
    } else if((l mod 3) == 0) {
	    i := i + 1;
    } else if((l mod 2) == 0) {
		  j := j+1;
    } else {
	    k := k+1;
    }
    l := l+1;
  }
  assert((i+j+k+v4) == l);
  
}

