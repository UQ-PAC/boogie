

procedure main() {
  // variable declarations
  var lock:int;
  var v1:int;
  var v2:int;
  var v3:int;
  var x:int;
  var y:int;
  
  // pre-conditions
  y := (x + 1);
  lock := 0;
  // loop body
  while ((x != y)) {
      
      if (*) {
        lock  :=  1;
        x  :=  y;
      } else {
        lock  :=  0;
        x  :=  y;
        y  :=  (y + 1);
      }
  }
  // post-condition
assert( (lock == 1) );
}
