procedure fib18()
{
	var b: int;
	var j: int;
	var flag: int;
	var n: int;

	assume(j == 0);
	assume(n > 0);
	assume(b == 0);

	while(b < n){
		if(flag == 1){
			j := j + 1;
		}

		b := b + 1;
	}

	assert((flag != 1) || (j == n));
}