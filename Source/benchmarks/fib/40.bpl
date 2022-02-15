procedure fib40()
{

	var i: int;
	var j: int;
	var k: int;
	var flag: int;

	var a: int;
	var b: int;

	var turn: int;
	
	assume(j == 1);
	assume((flag > 0 && i == 0) || (flag <= 0 && i == 1));
	assume(turn == 0);

	while(turn != 4){
		if(turn == 0){
      
			if(*){
				turn := 1;
			}
			else{
				turn := 2;
			}
		}

		if(turn == 1){
			i := i + 2;

			if(i mod 2 == 0){
				j := j + 2;
			}
			else{
				j := j + 1;
			}
			
			if(*){
				turn := 1;
			}
			else{
				turn := 2;
			}
		}
		else{
			if(turn == 2){
				a := 0;
				b := 0;

				turn := 3;
			}
			else{
				if(turn == 3){
					a := a + 1;
					b := b + j - i;
          
					if(*){
						turn := 3;
					}
					else{
						turn := 4;
					}
				}
			}
		}
	}

	assert(flag <= 0 || a == b);	
}