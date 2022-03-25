procedure main() {
  // variable declarations
  var i:int;
  var size:int;
  var sn:int;
  var v1:int;
  var v2:int;
  var v3:int;
  // pre-conditions
  sn := 0;
  i := 1;
  // loop body
  while ((i <= size)) {
    i  :=  (i + 1);
    sn  :=  (sn + 1);
  }
  // post-condition
  assert((sn != size) ==> (sn == 0) );
}
