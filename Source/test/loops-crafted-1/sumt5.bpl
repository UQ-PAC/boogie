procedure main() {
  var i: int, k: int, j: int, l: int, v3: int, v4: int, n: int;
  i := 0;
k := 0;
j := 0;
l := 0;
  v3 := 0;
 v4 := 0;
  assume(n >= 0);
  assume(n <= 20000001);
  while( l < n ) {
	
	  if((l mod 5) == 0) {
	    v3 := v3 + 1;
    } else if((l mod 4) == 0) {
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
  assert((i+j+k+v4+v3) == l);
  
}

