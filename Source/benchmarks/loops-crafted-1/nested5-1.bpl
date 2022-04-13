procedure main()
{
  var x: int;
  var y: int;
  var z: int;
  var w: int;
  var v: int;

  x := 0;
  y := 0;
  z := 0;
  w := 0;
  v := 0;

  while (w < 268435455) {
    x := 0;
    while (x < 10) {
      y := 0;
      while (y < 10) {
        z := 0;
        while (z < 10) {
          v := 0;
          while (v < 10) {

            v := v + 1;
          }
          assert(v mod 4 != 0);
          z := z + 1;
        }
        y := y + 1;
      }
      x := x + 1;
    }
    w := w + 1;
  }
}
