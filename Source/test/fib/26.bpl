procedure unknown() returns (u: bool);

procedure fib26()
{
  var u: bool;
	var w: int;
	var z: int;
	var x: int;
	var y: int;

	var turn: int;

	assume(w == 1);
	assume(z == 0);
	assume(x == 0);
	assume(y == 0);
	assume(turn == 0);

  call u := unknown();
	while(u){
		if(turn == 0){
      call u := unknown();
			if(u){
				turn := 1;
			}
			else{
				turn := 2;
			}
		}

		if(turn == 1){
			if(w mod 2 == 1){
				x := x + 1;
			}

			if(z mod 2 == 0){
				y := y + 1;
			}

			call u := unknown();
			if(u){
				turn := 1;
			}
			else{
				turn := 2;
			}
		}

		if(turn == 2){
			z := x + y;
			w := z + 1;
      call u := unknown();
			if(u){
				turn := 2;
			}
			else{
				turn := 0;
			}
		}

    call u := unknown();
	}

	assert(x == y);	
}