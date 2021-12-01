procedure unknown() returns (u: bool);

procedure main() {
  // variable declarations
  var x:int;
  var y:int;
  var u:bool;
  // pre-conditions
  assume((x >= 0));
  assume((x <= 10));
  assume((y <= 10));
  assume((y >= 0));
  // loop body
  call u := unknown();
  while (u) {
    x  :=  (x + 10);
    y  :=  (y + 10);
    call u := unknown();
  }
  // post-condition
  if (x == 20) {
    assert( y != 0 );
  }
}
