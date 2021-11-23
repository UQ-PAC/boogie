procedure fib44()
{
	var i: int;
	var j: int;
	var n: int;
	var k: int;

	assume(n == 1 || n == 2);
	assume(i == 0);
	assume(j == 0);

	while(i <= k){
		i := i + 1;
		j := j + n;
	}

	assert((n != 1) || (i == j));	
}