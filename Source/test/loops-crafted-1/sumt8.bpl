procedure main() {
  var n: int, i: int, k: int, j: int, l: int;
  var v1: int, v2: int, v3: int, v4: int, v5: int;
  i := 0;
  k := 0;
  j := 0;
  l := 0;
  v1 := 0;
  v2 := 0;
  v3 := 0;
  v4 := 0;
  v5 := 0;
  assume(n >= 0 && n <= 20000001);
  while( l < n ) {
	
	  if((l mod 8) == 0) {
      v5 := v5 + 1;
    } else if((l mod 7) == 0) {
	    v1 := v1 + 1;
    } else if((l mod 6) == 0) {
	    v2 := v2 + 1;
    } else if((l mod 5) == 0) {
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
    assert((i+j+k+v1+v2+v3+v4+v5) == l);
  }
  
}

