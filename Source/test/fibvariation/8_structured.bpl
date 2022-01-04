procedure fib8()
{
  var x: int, y: int, u: bool;
  assume(x == 0);
  assume(y == 0);

  while(u) {
    havoc u;
    if (u) {
      x := x + 1;
      y := y + 1;
    } else {
      havoc u;
      if (u) {
        if (x >= 4) {
          x := x + 1;
          y := y + 1;
        }
        if (x < 0) {
          y := y + 1;
        }
      }
    }
    havoc u;
  }
  assert(x < 4 || y > 2);
}