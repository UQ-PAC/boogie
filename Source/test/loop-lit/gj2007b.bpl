procedure main() {
    var n: int, x: int, m: int;
    var u: bool;
    x := 0;
    m := 0;
    while(x < n) {
	    havoc u;
  
      if(u) {
	      m := x;
	    }
	    x := x + 1;
    }
    assert((m >= 0 || n <= 0));
    assert((m < n || n <= 0));
}
