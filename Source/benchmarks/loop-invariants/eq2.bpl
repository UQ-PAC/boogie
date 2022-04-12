procedure main() {
  var w: int, x: int, y: int, z: int;
  assume (w >= 0);
  x := w;
  y := w + 1;
  z := x + 1;
  while(*) {
    y := y + 1;
    z := z + 1;
  }
  assert(y == z);
  
}
