procedure fib28()
{
  var n: int;
	var x: int;
	var y: int;

	assume(n == 0);
	assume(x >= 0);
	assume(y >= 0);
	assume(x == y);

	while(x != n){
		x := x - 1;
		y := y - 1;
	}

	assert(y == n);
}