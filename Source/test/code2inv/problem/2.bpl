procedure main() {
  // variable declarations
  var x:int;
  var y:int;
  // pre-conditions
  x := 1;
  y := 0;
  // loop body
  while ((y < 1000)) {
    x  :=  (x + y);
    y  :=  (y + 1);

  }
  // post-condition
assert( (x >= y) );
}