procedure unknown() returns (u: bool);

procedure main() {
  // variable declarations
  var c:int;
  var u:bool;
  // pre-conditions
  c := 0;
  // loop body
  call u := unknown();
  while (u) {
      call u := unknown();
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
      call u := unknown();
  }
  // post-condition
if ( (c != 40) ) {
  assert( (c <= 40) );
}
}
