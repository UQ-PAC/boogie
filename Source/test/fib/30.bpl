procedure unknown() returns (u: bool);

procedure fib30()
{
	var i: int;
	var c: int;
	var n: int;

	assume(i == 0);
	assume(c == 0);
	assume(n > 0);

	while(i < n){
		c := c + i;
		i := i + 1;
	}

	assert(c >= 0);	
}