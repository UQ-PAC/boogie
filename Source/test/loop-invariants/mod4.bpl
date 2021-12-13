procedure main() {
  var u: bool;
  var x: int;
  x := 0;
  while(u) {
    x := x + 4;
    havoc u;
  }
  assert((x mod 4) == 0);
  
}
