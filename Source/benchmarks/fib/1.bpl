procedure fib1() {
  var x: int, y: int, t1: int, t2: int;

  assume(x == 1);
  assume(y == 1);
  
  while(*){
    t1 := x;
	  t2 := y;
	  x := t1 + t2;
	  y := t1 + t2;
	  
  }
  assert(y >= 1);	
}