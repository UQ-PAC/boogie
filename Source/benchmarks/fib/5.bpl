procedure fib5(flag: bool)
{
  var i: int, j: int, x: int, y: int;
  i := 0;
  j := 0;
  x := 0;
  y := 0;
  
  while(*) {
    x := x + 1;
	  y := y + 1;
	  i := i + x;
	  j := j + y;
	  if (flag) {
	    j := j + 1;
	  }
    
  }
  assert(j>=i);
}