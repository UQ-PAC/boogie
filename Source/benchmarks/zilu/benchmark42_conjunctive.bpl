procedure main() {
  var x: int;
  var y: int;
  var z: int;
  assume(x == y && x >= 0 && x+y+z==0);
  while (x > 0) {
    x := x - 1;
    y := y - 1;
    z := z + 1;
    z := z + 1;
  }
  assert(z<=0);
  
}
