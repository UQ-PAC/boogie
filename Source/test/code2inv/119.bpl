procedure main() {
  // variable declarations
  var i:int;
  var size:int;
  var sn:int;
  // pre-conditions
  sn := 0;
  i := 1;
  // loop body
  while ((i <= size)) {
    i  :=  (i + 1);
    sn  :=  (sn + 1);
  }
  // post-condition
  assert((sn != 0) ==> (sn == size) );
}