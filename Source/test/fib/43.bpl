procedure unknown() returns (u: bool);

procedure fib43()
{
  var u: bool;
	var i: int;
	var t: int;
	var x: int;
	var y: int;

	assume(x != y);
	assume(i == 0);
	assume(t == y);

  //call u := unknown();
	while(u){
		if(x > 0){
			y := y + x;
		}
    havoc u;
	}

	assert(y >= t);	
}