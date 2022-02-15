

procedure main() {
  // variable declarations
  var c:int;
  
  // pre-conditions
  c := 0;
  // loop body
  while (*) {
      
      if (*) {
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
      
  }
  // post-condition

  assert((c != 4) ==> (c <= 4) );

}
