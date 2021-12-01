procedure main() {
  // variable declarations
  var n:int;
  var x:int;
  // pre-conditions
  x := n;
  // loop body
  while (((x > 0))) {
    x  := (x - 1);

  }
  // post-condition
if ( (x != 0) ) {
  assert( (n < 0) );
}
}
