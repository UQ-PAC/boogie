procedure fib5(flag: bool)
{
  var i: int, j: int, x: int, y: int, u: bool;
  i := 0;
  j := 0;
  x := 0;
  y := 0;
  
  while(u) {
    x := x + 1;
	  y := y + 1;
	  i := i + x;
	  j := j + y;
	  if (flag) {
	    j := j + 1;
	  }
    havoc u;
  }
  assert(j>=i);
}

procedure unknown() returns (u: bool);