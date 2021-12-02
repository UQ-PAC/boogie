procedure fib1() {
  var x: int, y: int, t1: int, t2: int;
  var u: bool;
  assume(x == 1);
  assume(y == 1);
  
  while(u){
    t1 := x;
	  t2 := y;
	  x := t1 + t2;
	  y := t1 + t2;
	  havoc u;
  }
  assert(y >= 1);	
}