procedure main() {
  // variable declarations
  var x:int;
  // pre-conditions
  x := 0;
  // loop body
  while (((x < 100))) {
    x := (x + 1);
  }
  // post-condition
assert( (x == 100) );
}
