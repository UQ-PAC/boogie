procedure unknown() returns (u: bool);

procedure main() {
  // variable declarations
  var c:int;
  var n:int;
  var u:bool;
  // pre-conditions
  c := 0;
  assume((n > 0));
  // loop body
  while (u) {
      havoc u;
      if (u) {
        if ( (c != n) )
        {
        c  :=  (c + 1);
        }
      } else {
        if ( (c == n) )
        {
        c  :=  1;
        }
      }
      havoc u;
  }
  // post-condition

  assert((c == n) ==> (n > -1) );
}
