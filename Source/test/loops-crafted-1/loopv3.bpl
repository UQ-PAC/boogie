procedure main() {
  var u: bool;
  var i: int, j: int;
  i := 0;
  j := 0;
  while(i<50000001){ 
    havoc u; 
    if(u) {	  
      i := i + 8; 
    } else {
     i := i + 4;    
    }
  }
  j := i div 4;
  assert( (j * 4) == i);
  
}
