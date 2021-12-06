procedure main() {
  // variable declarations
  var n:int;
  var x:int;
  // pre-conditions
  x := n;
  // loop body
  while (((x > 1))) {
    x  := (x - 1);

  }
  // post-condition
  assert( (x != 1) ==> (n < 0) );

}
