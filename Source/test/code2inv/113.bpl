procedure main() {
  // variable declarations
  var i:int;
  var n:int;
  var sn:int;
  var v1:int;
  var v2:int;
  var v3:int;
  // pre-conditions
  sn := 0;
  i := 1;
  // loop body
  while ((i <= n)) {
    i  :=  (i + 1);
    sn  :=  (sn + 1);
  }
  // post-condition
  assert((sn != 0) ==> (sn == n) );
}