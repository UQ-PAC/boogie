procedure main() {
  var x: int;
  var u: bool;
  assume((x>=0) && (x<=50));
  while(u) {
    if (x>50) {
      x := x + 1;
    } 
    if (x == 0) { 
      x := x + 1;
    } else {
      x := x - 1;
    }
    havoc u;
  }
  assert((x>=0) && (x<=50));
  
}
