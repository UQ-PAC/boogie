

procedure main() {
  // variable declarations
  var i:int;
  var n:int;
  var x:int;
  var y:int;
  
  // pre-conditions
  assume((n >= 0));
  i := 0;
  x := 0;
  y := 0;
  // loop body
  while ((i < n)) {
    
    i  :=  (i + 1);
    
      if (*) {
        x  :=  (x + 1);
        y  :=  (y + 2);
      } else {
        x  :=  (x + 2);
        y  :=  (y + 1);
      }

  }
  // post-condition
assert( ((3 * n) == (x + y)) );
}