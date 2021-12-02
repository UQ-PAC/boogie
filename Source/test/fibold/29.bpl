procedure unknown() returns (u: bool);

procedure fib29()
{
  var u: bool;
	var a: int;
	var b: int;
	var c: int;
	var d: int;
	var x: int;
	var y: int;
	var turn: int;

	assume(a == 1);
	assume(b == 1);
	assume(c == 2);
	assume(d == 2);
	assume(x == 3);
	assume(y == 3);
	assume(turn == 0);

  //call u := unknown();
	while(u){
		if(turn == 0){
			x := a + c;
			y := b + d;
		
			if((x + y) mod 2 == 0){
				a := a + 1;
				d := d + 1;
			}
			else{
				a := a - 1;
			}

			turn := 1;
		}
		else{
			if(turn == 1){
				c := c - 1;
				b := b - 1;
				if(u){
					turn := 1;
				}
				else{
					turn := 0;
				}
			}

		}
    havoc u;
	}

	assert(a + c == b + d);	
}