procedure fib4() {
  var x: int, y: int;

  x := -50;
  while(x < 0) {
	  x := x + y;
	  y := y + 1;
  }
  assert(y>0);
}