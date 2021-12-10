procedure main() {
  var x: int;
  var y: int;
  var t: int;
  var u: bool;
  
  assume(x!=y && y==t);
  while(u) {
    if(x>0){
      y := y+x;
    }
    havoc u;
  }
  assert(y>=t);
  
}
