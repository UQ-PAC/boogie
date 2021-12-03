procedure unknown() returns (u: bool);

procedure main() {
  // variable declarations
  var x:int;
  var y:int;
  var u:bool;
  // pre-conditions
  assume((x >= 0));
  assume((x <= 2));
  assume((y <= 2));
  assume((y >= 0));
  // loop body
  while (u) {
    x  :=  (x + 2);
    y  :=  (y + 2);
    havoc u;
  }
  // post-condition
  assert( (y == 0) ==> (x != 4) );
}
