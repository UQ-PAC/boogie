procedure fib36()
{
  var u: bool;
	var a: int;
	var b: int;
	var x: int;
	var y: int;
	var z: int;
	var w: int;
	var flag: int;

	var i: int;
	var j: int;
	var k: int;

	var c: int;
	var d: int;

	var turn: int;


	assume(a == 0);
	assume(b == 0);
	assume(x == 0);
	assume(y == 0);
	assume(z == 0);
	assume(j == 0);
	assume(w == 0);
	assume(turn == 0);

	while(u){
		if(turn == 0){
      havoc u;
			if(u){
				turn := 1;
			}
			else{
				turn := 6;
			}
		}

		if(turn == 1){
			i := z;
			j := w;
			k := 0;
			
			turn := 2;
		}

		if(turn == 2){
			if(i < j){	
				k := k + 1;
				i := i + 1;
			}
			else{
				turn := 3;
			}
		}
		else{
			if(turn == 3){
				x := z;
				y := k;

				if(x mod 2 == 1){
					x := x + 1;
					y := y - 1;
				}
				else{
					turn := 4;
				}
			}
			else{
				if(turn == 4){
					if(x mod 2 == 0){
						x := x + 2;
						y := y - 2;
					}
					else{
						x := x - 1;
						y := y - 1;
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
						z := z + 1;
						w := x + y + 1;
					
						turn := 0;
					}
				}
			}
		}

		if(turn == 6){
			c := 0;
			d := 0;

			turn := 7;
		}
		else{
			if(turn == 7){
				c := c + 1;
				d := d + 1;

				
				if(flag > 0){
					a := a + 1;
					b := b + 1;
				}
				else{
					a := a + c;
					b := b + d;
				}
        havoc u;
				if(u){
					turn := 7;				
				}
				else{
					turn := 0;
				}
			}
		}
    havoc u;
	}

	assert(w >= z && a == b);	
}