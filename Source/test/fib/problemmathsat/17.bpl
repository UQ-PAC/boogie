procedure fib17()
{
	var k: int;
	var i: int;
	var j: int;
	var n: int;

	var turn: int;

	assume(k == 1);
	assume(i == 1);
	assume(j == 0);
	assume(turn == 0);

	while(turn != 3){
		if(turn == 0){
			if(i < n){
				j := 0;
				turn := 1;
			}
			else{
				turn := 3;
			}
			
		}

		if(turn == 1){
			if(j < i){
				k := k + i - j;
				j := j + 1;
			}
			else{
				turn := 2;
			}
			
		}

		if(turn == 2){
			i := i + 1;
			turn := 0;
		}
		
	}

	assert(k >= n);	
}