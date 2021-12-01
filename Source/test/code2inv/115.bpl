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
  call u := unknown();
  while (u) {
    x  :=  (x + 1);
    sn  :=  (sn + 1);
    call u := unknown();
  }
  // post-condition
if ( (sn != -1) ) {
  assert( (sn == x) );
}
}
