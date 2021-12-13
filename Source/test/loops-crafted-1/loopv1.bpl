procedure main() {
  var u: bool;
  var n: int, i: int, j: int;
  assume(n <= 50000001);
  i := 0;
  j := 0;
  while(i<n){ 
    havoc u; 
    if(u) {	  
      i := i + 6; 
    } else {
      i := i + 3;    
    }
  }
  assert( (i mod 3) == 0 );
  
}
