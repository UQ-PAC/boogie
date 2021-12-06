procedure fib32()
{
	var k: int;
	var b: int;
	var i: int;
	var j: int;
	var n: int;

	assume(k > 0);
	assume(i == j);
	assume(n == 0);
	assume((b == 1) || (b == 0));

	while(n < 2*k){
		if(b == 1){
			i := i + 1;
			b := 0;
		}
		else{
			j := j + 1;
			b := 1;
		}

		n := n + 1;
	}

	assert(i == j);	
}