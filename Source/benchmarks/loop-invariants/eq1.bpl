procedure main() {
  var w: int;
  var x: int, y: int;
  var z: int;
  assume (w >= 0);
  x := w;
  assume (y >= 0);
  z := y;
  while(*) {
    if (*) {
      w := w + 1;
      x := x + 1;
    } else {
      y := y - 1;
      z := z - 1;
    }
  }
  assert(w == x && y == z);
  
}
