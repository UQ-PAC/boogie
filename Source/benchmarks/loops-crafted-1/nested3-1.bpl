procedure main()
{
  var x: int;
  var y: int;
  var z: int;
  var w: int;

  x := 0;
  y := 0;
  z := 0;
  w := 0;

  while (x < 268435455) {
    y := 0;

    while (y < 268435455) {
   	  z := 0;
	    while (z < 268435455) {
	      z := z + 1;
	    }
      assert(z mod 4 != 0);
	    y := y + 1;
    }
    assert(y mod 2 != 0);
    x := x + 1;
  }
  assert(x mod 2 != 0);

}
