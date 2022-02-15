procedure main() {
  var x: int;
  
  assume(x==1 || x==2);
  while(*) {
    if(x==1) {
      x := 2;
    } else if (x==2) {
      x := 1;
    } 
    
  }
  assert(x<=8);
  
}
