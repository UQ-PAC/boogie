procedure fib38()
{
  var i: int;
	var x: int;
	var y: int;
	var n: int;

	assume(i == 0);
	assume(x == 0);
	assume(y == 0);

	while(i < n){
		i := i + 1;
		x := x + 1;
		
		if(i mod 2 == 0){
			y := y + 1;
		}
	}

	assert((i mod 2 != 0) || (x == 2 * y));
}