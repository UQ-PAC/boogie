procedure unknown() returns (u: bool);

procedure main() {
  // variable declarations
  var sn:int;
  var v1:int;
  var v2:int;
  var v3:int;
  var x:int;
  var u:bool;
  // pre-conditions
  sn := 0;
  x := 0;
  // loop body
  while (u) {
    x  :=  (x + 1);
    sn  :=  (sn + 1);
    havoc u;
  }
  // post-condition
  assert((sn != x) ==> (sn == -1) );
}
