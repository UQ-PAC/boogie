procedure main() {
  var x: int;
  var y: int;
  var z: int;
  var u: bool;
  assume(x == y && y == 0 && z==0);
  while(u) {
    x := x + 1;
    y := y + 1;
    z := z - 2;
    havoc u;
  }
  assert(x == y && x >= 0 && x+y+z==0);
  
}
