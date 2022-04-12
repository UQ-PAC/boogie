procedure hola25()
{

	var x: int;
	var y: int;
	var i: int;
	var j: int;
  x := 0;
  y := 0;
  i := 0;
  j := 0;
  while (*) {
    while (*) {
      if(x == y){
				i := i + 1;
			} else{
				j := j + 1;
			}
    }
    if(i >= j){
			x := x + 1;
			y := y + 1;
		} else{
		  y := y + 1;
		}
  }
  assert (i >= j);
}