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
  assume((x <= 10));
  assume((y <= 10));
  assume((y >= 0));
  // loop body
  while (u) {
    x  :=  (x + 10);
    y  :=  (y + 10);
    havoc u;
  }
  // post-condition
    assert( (x == 20) ==> (y != 0) );
}
