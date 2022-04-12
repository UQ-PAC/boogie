procedure hola45()
{

	var x: int;
	var y: int;
	var i: int;
	var j: int;
	var c: int;
	var d: int;
	var flag: int;

	var w: int;
	var z: int;

  assume(x == 0);
	assume(y == 0);
	assume(i == 0);
	assume(j == 0);
	assume(c == 0);
	assume(d == 1);

  while (*) {
    x := x + 1;
		y := y + 1;
		i := i + x;
		j := j + y;

		if(flag > 0){
			j := j + 1;
		}
  }
  if(j >= i){
    x := y;
  }
  else{
    x := y + 1;
  }

  w := 1;
  z := 0;
  while (*) {
    while (*) {
      if(w mod 2 == 1){
        x := x + 1;
      }

      if(z mod 2 == 0){
        y := y + 1;
      }
    }
    z := x + y;
		w := z + 1;
  }
  assert (x == y);
}