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
        if ( (c != 40) )
        {
        c  :=  (c + 1);
        }
      } else {
        if ( (c == 40) )
        {
        c  :=  1;
        }
      }
      havoc u;
  }
  // post-condition

  assert((c != 40) ==> (c <= 40) );
}
