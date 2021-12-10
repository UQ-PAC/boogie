procedure main() {
  var x: int;
  var y: int;
  var u: bool;
  assume(y == x);
  while(u) {
    x := x + 1;
    y := y + 1;
    havoc u;
  }
  assert(x == y);
  
}
