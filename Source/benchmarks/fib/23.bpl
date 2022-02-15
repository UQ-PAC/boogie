procedure fib23()
{

	var i: int;
	var sum: int;
	var n: int;

	assume(sum == 0);
	assume(n >= 0);
	assume(i == 0);

	while(i < n){
		sum := sum + i;
		i := i + 1;
	}

	assert(sum >= 0);	
}