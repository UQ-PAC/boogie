procedure fib19()
{
  var n: int;
	var m: int;
	var x: int;
	var y: int;

	assume(n >= 0);
	assume(m >= 0);
	assume(m < n);
	assume(x == 0);
	assume(y == m);

	while(x < n){
		x := x + 1;
		
		if(x > m){
			y := y + 1;
		}
	}

	assert(y == n);
}