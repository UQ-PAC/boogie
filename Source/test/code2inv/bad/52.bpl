procedure unknown() returns (u: bool);

procedure main() {
  // variable declarations
  var c:int;
  var u:bool;
  // pre-conditions
  c := 0;
  // loop body
  while (u) {
      havoc u;
      if (u) {
        if ( (c != 4) )
        {
        c  :=  (c + 1);
        }
      } else {
        if ( (c == 4) )
        {
        c  :=  1;
        }
      }
      havoc u;
  }
  // post-condition
if ( (c < 0) ) {
  if ( (c > 4) ) {
    assert( (c == 4) );
  }
}  
}
