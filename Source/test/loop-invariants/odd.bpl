procedure main() {
  var u: bool;
  var x: int;
  x := 1;
  while(u) {
    x := x + 2;
    havoc u;
  }
  assert((x mod 2) != 0);
  
}