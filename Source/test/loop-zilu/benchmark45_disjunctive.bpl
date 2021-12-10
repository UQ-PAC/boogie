procedure main() {
  var x: int;
  var y: int;
  var u: bool;
  assume(y>0 || x>0);
  while(u) {
    if (x>0) {
      x := x + 1;
    } else {
      y := y + 1;
    }
    havoc u;
  }
  assert(x>0 || y>0);
  
}
