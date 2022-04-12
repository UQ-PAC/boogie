procedure main() {
  var x: int, y: int, z: int;
  x := 0;
  y := 10000000;
  z := 0;
	while(x<y){	
		if(x>=5000000) {
			z := z+2;
    }
		x := x + 1;
	}
  assert((z mod 2) == 0);
  
}
