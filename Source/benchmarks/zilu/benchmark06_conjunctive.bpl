procedure main() {
  var j: int;
  var i: int;
  var x: int;
  var y: int;
  var k: int;
  
  j := 0;
  assume(x+y==k);
  while(*) {
    if(j==i) {
      x := x + 1;
      y := y - 1;
    } else {
      y := y + 1;
      x := x - 1;
    }
    j := j + 1;
    
  }
  assert(x+y==k);
  
}
