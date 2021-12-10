procedure main() {
  var x: int;
  var u: bool;
  assume(x==1 || x==2);
  while(u) {
    if(x==1) {
      x := 2;
    } else if (x==2) {
      x := 1;
    } 
    havoc u;
  }
  assert(x<=8);
  
}
