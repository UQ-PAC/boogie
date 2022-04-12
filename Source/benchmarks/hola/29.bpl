procedure hola29()
{

	var a: int;
	var b: int;
	var c: int;
	var d: int;
	var x: int;
	var y: int;
  a := 1;
  b := 1;
  c := 2;
  d := 2;
  x := 3;
  y := 3;
  while (*) {
    x := a + c;
    y := b + d;
  
    if((x + y) mod 2 == 0){
      a := a + 1;
      d := d + 1;
    }
    else{
      a := a - 1;
    }
    while (*) {
      c := c - 1;
      b := b - 1;
    }
  }
  assert (a+c == b+d);
}