procedure main() {
  // variable declarations
  var n:int;
  var v1:int;
  var v2:int;
  var v3:int;
  var x:int;
  // pre-conditions
  x := 0;
  // loop body
  while (((x < n))) {
    x := (x + 1);
  }
  // post-condition
  assert((n >= 0) ==> (x == n) );
}