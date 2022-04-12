procedure hola33()
{

	var x: int;
	var y: int;
	var z: int;
	var c: int;
	var k: int;
  z := k;
  x := 0;
  y := 0;
  while (*) {
    c := 0;
    while (*) {
      if(z == k + y - c){
				x := x + 1;
				y := y + 1;
				c := c + 1;
			} else{
				x := x + 1;
				y := y - 1;
				c := c + 1;
			}
    }
    while (*) {
      x := x - 1;
      y := y - 1;
    }
    z := k + y;
  }
  assert (x == y);
}