procedure main()
{
  var SIZE: int;
  var last: int;

  var a: int;
  var b: int;
  var c: int;
  var d: int;
  var st: int;

  assume (last > 0);
  SIZE := 200000;

	while(true) {
		st := 1;
    
    c := 0;
    while (c < SIZE) {
      if (c >= last) {
        st := 0;
      }
      c := c + 1;
    }
		if(st==0 && c==last+1) {
			a := a + 3;
      b := b + 3;
    } else {
			a := a + 2;
      b := b + 2;
    } 
		if(c==last && st==0) { 
			a := a+1;
		} else if(st==1 && last<SIZE) { 
			d := d + 1;
		}
		if(d == SIZE) {
			a := 0; 
			b := 1;
		}
			
		assert(a==b && c==SIZE);
	}

}
