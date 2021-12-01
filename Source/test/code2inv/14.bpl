procedure unknown() returns (u: bool);

procedure main() {
  // variable declarations
  var x:int;
  var y:int;
  var u:bool;
  var z1:int;
  var z2:int;
  var z3:int;
  // pre-conditions
  assume((x >= 0));
  assume((x <= 2));
  assume((y <= 2));
  assume((y >= 0));
  // loop body
  call u := unknown();
  while (u) {
    x  :=  (x + 2);
    y  :=  (y + 2);
    call u := unknown();
  }
  // post-condition
  if ( (y == 0) ) {
    assert( (x != 4) );
  }
}
