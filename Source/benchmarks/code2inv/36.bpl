

procedure main() {
  // variable declarations
  var c:int;
  
  // pre-conditions
  c := 0;
  // loop body
  while (*) {
      
      if (*) {
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
      
  }
  // post-condition

  assert((c != 40) ==> (c <= 40) );
}
