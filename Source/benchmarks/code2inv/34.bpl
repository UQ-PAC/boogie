procedure main() {
  // variable declarations
  var n:int;
  var v1:int;
  var v2:int;
  var v3:int;
  var x:int;
  // pre-conditions
  x := n;
  // loop body
  while (((x > 0))) {
    x := (x - 1);

  }
  // post-condition

  assert((n >= 0) ==> (x == 0) );
}
