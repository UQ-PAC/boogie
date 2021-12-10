procedure main() {
  var x: int;
  var y: int;
  var u: bool;
  assume(x == y && y == 0);
  while(u) {
    x := x + 1;
    y := y + 1;
    havoc u;
  }
  assert(x == y && x >= 0);
  
}
