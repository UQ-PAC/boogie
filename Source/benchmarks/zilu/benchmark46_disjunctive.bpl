procedure main() {
  var x: int;
  var y: int;
  var z: int;
  
  assume(y>0 || x>0 || z>0);
  while(*) {
    if (x>0) {
      x := x + 1;
    }
    if (y>0) {
      y := y + 1;
    } else {
      z := z + 1;
    }
    
  }
  assert(x>0 || y>0 || z>0);
  
}
