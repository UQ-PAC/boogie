procedure main() {
  // variable declarations
  var i:int;
  var j:int;
  var x:int;
  var y:int;
  // pre-conditions
  i := x;
  j := y;
  // loop body
  while ((x != 0)) {
    x  :=  (x - 1);
    y  :=  (y - 1);
  }
  // post-condition
  assert((y != 0) ==> (i != j) );
}
