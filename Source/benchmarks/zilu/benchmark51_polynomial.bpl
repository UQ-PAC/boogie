procedure main() {
  var x: int;
  
  assume((x>=0) && (x<=50));
  while(*) {
    if (x>50) {
      x := x + 1;
    } 
    if (x == 0) { 
      x := x + 1;
    } else {
      x := x - 1;
    }
    
  }
  assert((x>=0) && (x<=50));
  
}
