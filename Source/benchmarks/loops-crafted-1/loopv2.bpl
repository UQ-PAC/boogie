procedure main() {
  var n: int;
  var i: int;
  var j: int;
  var k: int;
  assume(n <= 50000001);
  i := 0;
  j := 0;
  while(i<n){ 
    i := i + 4;
    j := i + 2;    
  }
  k := i;
  while( (j mod 2) == 0) {
    j := j - 4;
    k := k - 4; 
  }
  assert( (k mod 2) == 0 );

}
