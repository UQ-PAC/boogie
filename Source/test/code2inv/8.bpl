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
  while (u) {
    x  :=  (x + 10);
    y  :=  (y + 10);
    havoc u;
  }
  // post-condition
    assert((y == 0) ==> (x != 20) );

}
