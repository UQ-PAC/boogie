procedure main()
{
	var x: int, y: int, z: int;
  x := 0;
  y := 500000;
  z := 0;
	while(x<1000000){
		if(x<500000) {
			x := x + 1;
    }	else{
			if(x<750000) {
				x := x + 1;
			}
			else{
				x := x+2;
			}
			y := y + 1;
		}
	}
	 assert(x==1000000);
	
}
