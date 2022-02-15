

procedure main() {
  // variable declarations
  var i:int;
  var x:int;
  var y:int;
  
  // pre-conditions
  i := 0;
  assume((x >= 0));
  assume((y >= 0));
  assume((x >= y));
  // loop body
  while (*) {
    if ( (i < y) )
    {
    i  :=  (i + 1);
    }
    
  }
  // post-condition
if ( (i < y) ) {
  assert( (i < x) );
}
}
