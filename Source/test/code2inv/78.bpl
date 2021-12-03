procedure unknown() returns (u: bool);

procedure main() {
  // variable declarations
  var i:int;
  var x:int;
  var y:int;
  var u:bool;
  // pre-conditions
  i := 0;
  assume((x >= 0));
  assume((y >= 0));
  assume((x >= y));
  // loop body
  while (u) {
    if ( (i < y) )
    {
    i  :=  (i + 1);
    }
    havoc u;
  }
  // post-condition
if ( (i < y) ) {
  assert( (0 <= i) );
}
}
