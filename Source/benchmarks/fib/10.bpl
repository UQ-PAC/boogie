procedure fib10()
{
  var x: int;
  var y: int;
  var w: int;
  var z: int;
  

  assume(w == 1);
  assume(z == 0);
  assume(x == 0);
  assume(y == 0);

  while(*) {
    if (w == 1) {
        x := x + 1;
        w := 0;
    }
    if (z == 0) {
        y := y + 1;
        z := 1;
    }
    
  }
  assert(x == y);
}
