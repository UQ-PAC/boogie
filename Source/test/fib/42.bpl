procedure fib42()
{
  var u: bool;
	var x: int;
	var y: int;
	var a: int;
	var flag: int;

	assume(x == 1);
	assume(y == 1);
	assume((flag == 1 && a == 0) || (flag != 1 && a == 1));

	//call u := unknown();
	while(u){
		if(flag == 1) {
			a := x + y;
			x := x + 1;
		} 
		else {
      a := x + y + 1;
			y := y + 1;
  	}

		if(a mod 2 == 1){
			y := y + 1;
		}
		else{
			x := x + 1;
		}
		havoc u;
	}

	assert((flag != 1) || (a mod 2 == 0));	
}