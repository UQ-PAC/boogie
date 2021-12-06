procedure unknown() returns (u: bool);

procedure fib37()
{
  var u: bool;
	var x: int;
	var m: int;
	var n: int;

	assume(x == 0);
	assume(m == 0);
	assume(n > 0);

	while(x < n){
    havoc u;
		if(u){
			m := x;		
		}

		x := x + 1;
	}

	assert((n <= 0) || (0 <= m && m < n));	
}