procedure hola36()
{

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

  a := 0;
  b := 0;
  x := 0;
  y := 0;
  z := 0;
  j := 0;
  w := 0;

  while (*) {
    i := z;
    j := w;
    k := 0;
    while (i < j) {
      k := k + 1;
			i := i + 1;
    }
    x := z;
		y := k;
    if(x mod 2 == 1){
		  x := x + 1;
			y := y - 1;
		}
    while (*) {
      if(x mod 2 == 0){
				x := x + 2;
				y := y - 2;
			} else{
				x := x - 1;
				y := y - 1;
			}
    }
    z := z + 1;
		w := x + y + 1;
  }
  c := 0;
  d := 0;
  while (*) {
    c := c + 1;
		d := d + 1;
    if(flag != 0){
      a := a + 1;
      b := b + 1;
    } else{
      a := a + c;
      b := b + d;
    }
  }
  assert (w >= z && a-b == 0);
}
