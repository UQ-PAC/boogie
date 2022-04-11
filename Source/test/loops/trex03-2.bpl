procedure main()
{
  var x1: int, x2: int, x3: int;
  var d1: int, d2: int, d3: int;
  assume (x1 >= 0 && x2 >= 0 && x3 >= 0);
  d1 := 1;
  d2 := 1;
  d3 := 1;
  
  while(x1>0 && x2>0 && x3>0)
  {
    if (*) {
      x1 := x1-d1;
    }
    else if (*) {
      x2 := x2-d2;
    }
    else {
      x3 := x3-d3;
    }
  }

  assert(x1==0 || x2==0 || x3==0);
  
}

