procedure fib45()
{
  var u: bool;
	var x: int;
	var y: int;
	var i: int;
	var j: int;
	var c: int;
	var d: int;
	var flag: int;

	var w: int;
	var z: int;

	var turn: int;

	assume(x == 0);
	assume(y == 0);
	assume(i == 0);
	assume(j == 0);
	assume(c == 0);
	assume(d == 1);
	assume(turn == 0);

	while(turn != 6){
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
			x := x + 1;
			y := y + 1;
			i := i + x;
			j := j + y;

			if(flag > 0){
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
				if(j >= i){
					x := y;
				}
				else{
					x := y + 1;
				}

				w := 1;
				z := 0;

				turn := 3;
			}
		}

		if(turn == 3){
			turn := 4;
		}
		else{
			if(turn == 4){
				if(w mod 2 == 1){
					x := x + 1;
				}

				if(z mod 2 == 0){
					y := y + 1;
				}
				havoc u;
				if(u){
					turn := 4;
				}
				else{
					turn := 5;
				}
			}
			else{
				if(turn == 5){
					z := x + y;
					w := z + 1;
          havoc u;
					if(u){
						turn := 3;
					}
					else{
						turn := 6;
					}
				}
			}
		}
	}

	assert(x == y);	
}