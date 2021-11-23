procedure fib2()
{
  var i: int, j: int, z: int, x: int, y: int, w: int, u: bool;
  i := 1;
  j := 0;
  z := i - j;
  x := 0;
  y := 0;
  w := 0;
  
  call u := unknown();
  while(u) {
    z := z + x + y + w;
    y := y + 1;
	  if(z mod 2==1) {
	    x := x + 1;
	  }
	  w := w + 2;
    call u := unknown();	
  }
  assert(x==y);
}

procedure unknown() returns (u: bool);