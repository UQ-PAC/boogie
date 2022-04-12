procedure hola26()
{

	var w: int;
	var z: int;
	var x: int;
	var y: int;
  w := 1;
  z := 0;
  x := 0;
  y := 0;
  while (*) {
    while (*) {
      if(w mod 2 == 1){
				x := x + 1;
			}

			if(z mod 2 == 0){
				y := y + 1;
			}
    }
    while (*) {
      z := x + y;
			w := z + 1;
    }
  }
  assert (x == y);
}