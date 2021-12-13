procedure main() {
  var w: int, x: int, y: int, z: int;
  var u: bool;
 assume (w >= 0);
  x := w;
  y := w + 1;
  z := x + 1;
  while(u) {
    y := y + 1;
    z := z + 1;
    havoc u;
  }
  assert(y == z);
  
}
