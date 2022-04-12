procedure main()
{
  var x: int, y: int, z: int;
	x := 0;
	y := 10000000;
	z := 5000000;
	while(x<y){	
		if(x>=5000000) {
			z := z + 1;
    }

		x := x + 1;
	}
	assert(z==x);
	
}
