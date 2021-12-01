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
  call u := unknown();
  while (u) {
      call u := unknown();  
      if (u) {
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
      call u := unknown();
  }
  // post-condition
if ( (c == n) ) {
  assert( (n > -1) );
}
}
