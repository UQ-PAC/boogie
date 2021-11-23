procedure fib41()
{
	var n: int;
	var k: int;
	var i: int;
	var j: int;

	assume(j == 0);
	assume(n >= 0);
	assume(i == 0);
	assume(k == 1 || k >= 0);

	while(i <= n){
		i := i + 1;
		j := j + i;
	}

	assert(k + i + j > 2 * n);
}