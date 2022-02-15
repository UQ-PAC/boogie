procedure main() {
  // variable declarations
  var n:int;
  var x:int;
  // pre-conditions
  x := 0;
  // loop body
  while (((x < n))) {
    x := (x + 1);

  }
  // post-condition
  assert((x != n) ==> (n < 0) );

}
