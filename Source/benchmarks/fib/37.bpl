procedure fib37()
{

	var x: int;
	var m: int;
	var n: int;

	assume(x == 0);
	assume(m == 0);
	assume(n > 0);

	while(x < n){
    
		if(*){
			m := x;		
		}

		x := x + 1;
	}

	assert((n <= 0) || (0 <= m && m < n));	
}