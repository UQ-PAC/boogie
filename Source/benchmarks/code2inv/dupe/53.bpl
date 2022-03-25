

procedure main() {
  // variable declarations
  var c:int;
  var n:int;
  var v1:int;
  var v2:int;
  var v3:int;
  
  // pre-conditions
  c := 0;
  assume((n > 0));
  // loop body
  while (*) {
      
      if (*) {
        if ( (c > n) )
        {
        c  :=  (c + 1);
        }
      } else {
        if ( (c == n) )
        {
        c  :=  1;
        }
      }
      
  }
  // post-condition

  assert((c != n) ==> (c >= 0) );
}
