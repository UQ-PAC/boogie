

procedure main() {
  // variable declarations
  var sn:int;
  var v1:int;
  var v2:int;
  var v3:int;
  var x:int;
  
  // pre-conditions
  sn := 0;
  x := 0;
  // loop body
  while (*) {
    x  :=  (x + 1);
    sn  :=  (sn + 1);
    
  }
  // post-condition
  assert((sn != -1) ==> (sn == x) );
}
