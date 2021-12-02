procedure unknown() returns (u: bool);

procedure fib22()
{
  var u: bool;
  var x: int;
	var y: int;
	var z: int;
	var k: int;

	assume(x == 0);
	assume(y == 0);
	assume(z == 0);
	assume(k == 0);

	while(u){
		if(k mod 3 == 0){
			x := x + 1;
		}

		y := y + 1;
		z := z + 1;
		k := x + y + z;
    havoc u;
	}

	assert(x == y);
	assert(y == z);
}