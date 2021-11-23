procedure fib35()
{
	var x: int;
	var n: int;

	assume(n > 0);
	assume(x == 0);

	while(x < n){
		x := x + 1;
	}

	assert(x == n);	
}