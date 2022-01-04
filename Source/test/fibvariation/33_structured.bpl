procedure fib33()
{
  var u: bool;
	var x: int;
	var y: int;
	var z: int;
	var c: int;
	var k: int;

	var turn: int;

	assume(z == k);
	assume(x == 0);
	assume(y == 0);
	assume(turn == 0);

	while(u){
		if(turn == 0){
			c := 0;
			havoc u;
			if(u){
				turn := 1;
			}
			else{
				turn := 2;
			}
		}

		if(turn == 1){
			if(z == k + y - c){
				x := x + 1;
				y := y + 1;
				c := c + 1;
			}
			else{
				x := x + 1;
				y := y - 1;
				c := c + 1;
			}
			havoc u;
			if(u){
				turn := 1;
			}
			else{
				turn := 2;
			}	
		}
		else{
			if(turn == 2){
	
				x := x - 1;
				y := y - 1;
        havoc u;
				if(u){
					turn := 2;
				}
				else{
					turn := 3;
				}
			}
			else{
				z := k + y;
				turn := 0;
			}
		}
    havoc u;
	}

	assert(x == y);	
}