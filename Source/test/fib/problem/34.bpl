procedure fib34()
{
	var x: int;
	var y: int;
	var i: int;
	var m: int;

	assume(x == 0);
	assume(y == 0);
	assume(i == 0);
	assume(m == 10);

	while(i < m){
		i := i + 1;
		x := x + 1;

		if(x mod 2 == 0){
			y := y + 1;		
		}

	}
	
	assert((i != m) || (x == 2*y));	
}