procedure unknown() returns (u: bool);

procedure fib25()
{
  var u: bool;
	var x: int;
	var y: int;
	var i: int;
	var j: int;

	var turn: int;

	assume(x == 0);
	assume(y == 0);
	assume(i == 0);
	assume(j == 0);
	assume(turn == 0);

	while(u){
		if(turn == 0){
      havoc u;
			if(u){
				turn := 1;
			}
			else{
				turn := 2;
			}
		}


		if(turn == 1){
			if(x == y){
				i := i + 1;
			}
			else{
				j := j + 1;
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
				if(i >= j){
					x := x + 1;
					y := y + 1;
				}
				else{
					y := y + 1;
				}

				turn := 0;
			}
		}
    havoc u;
	}

	assert(i >= j);	
}