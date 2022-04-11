procedure hola6()
{
  var w: int, z: int, x: int, y: int;
  w := 1;
  z := 0;
  x := 0;
  y := 0;

  while(*){
	  
    while(*){
      if((w mod 2) == 1) {
        x := x + 1;
      } 
      if((z mod 2) == 0) {
        y := y + 1;
      }
    }
    z := x+y;
    w := z+1;
  }

  
  assert(x==y);
}
