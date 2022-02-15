

procedure main() {
  // variable declarations
  var x:int;
  var y:int;
  
  // pre-conditions
  assume((x >= 0));
  assume((x <= 2));
  assume((y <= 2));
  assume((y >= 0));
  // loop body
  while (*) {
    x  :=  (x + 2);
    y  :=  (y + 2);
    
  }
  // post-condition
    assert((x == 4) ==> (y != 0) );
}
