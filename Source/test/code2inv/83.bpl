procedure main() {
  // variable declarations
  var x:int;
  var y:int;
  // pre-conditions
  x := -5000;
  // loop body
  while ((x < 0)) {
    x  :=  (x + y);
    y  :=  (y + 1);
  }
  // post-condition
assert( (y > 0) );
}