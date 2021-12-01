procedure unknown() returns (u: bool);

procedure main() {
  // variable declarations
  var c:int;
  var x1:int;
  var x2:int;
  var x3:int;
  var y:int;
  var z:int;
  var u:bool;
  // pre-conditions
  c := 0;
  assume((y >= 0));
  assume((y >= 127));
  z := (36 * y);
  // loop body
  call u := unknown();
  while (u) {
    if ( (c < 36) )
    {
    z  :=  (z + 1);
    c  :=  (c + 1);
    }
    call u := unknown();
  }
  // post-condition
if ( (z < 0) ) {
  if ( (z >= 4608) ) {
    assert( (c >= 36) );
  }
}  
}
