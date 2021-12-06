procedure fib20()
{
	var u: bool;
	var x: int;
	var y: int;
	var k: int;
	var j: int;
	var i: int;
	var n: int;
	var m: int;

	assume(x+y == k);
	assume(m == 0);
	assume(j == 0);

	while(j < n){
		if(j == i){
			x := x + 1;
			y := y - 1;
		}
		else{
			x := x - 1;
			y := y + 1;
		}

    havoc u;
		if(u){
			m := j;
		}

		j := j + 1;
	}

	assert(x + y == k);
	assert((n <= 0) || (0 <= m));
	assert((n <= 0) || (m <= n));
}