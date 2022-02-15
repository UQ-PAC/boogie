procedure main() {
  var x: int;
  var y: int;
  var z: int;
  
  assume(x == y && y == 0 && z==0);
  while(*) {
    x := x + 1;
    y := y + 1;
    z := z - 2;
    
  }
  assert(x == y && x >= 0 && x+y+z==0);
  
}
