procedure main() {
  // variable declarations
  var i:int;
  var j:int;
  // pre-conditions
  i := 1;
  j := 20;
  // loop body
  while ((j >= i)) {
    i  :=  (i + 2);
    j  :=  (j - 1);

  }
  // post-condition
assert( (j == 13) );
}
