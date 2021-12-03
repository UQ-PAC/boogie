procedure unknown() returns (u: bool);

procedure main() {
  // variable declarations
  var sn:int;
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
  assert((sn != -1) ==> (sn == x) );
}
