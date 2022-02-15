procedure fib8()
{
  var x: int, y: int;
  assume(x == 0);
  assume(y == 0);

  while(*) {
    
    if (*) {
      x := x + 1;
      y := y + 1;
    } else {
      
      if (*) {
        if (x >= 4) {
          x := x + 1;
          y := y + 1;
        }
        if (x < 0) {
          y := y + 1;
        }
      }
    }
    
  }
  assert(x < 4 || y > 2);
}