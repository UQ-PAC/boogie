procedure main() {
  var x: int;
  var y: int;
  var t: int;
  
  
  assume(x!=y && y==t);
  while(*) {
    if(x>0){
      y := y+x;
    }
    
  }
  assert(y>=t);
  
}
