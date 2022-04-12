procedure main() {
  var i: int, j: int;
  i := 0;
  j := 0;
  while(i<50000001){ 
    if(*) {	  
      i := i + 8; 
    } else {
     i := i + 4;    
    }
  }
  j := i div 4;
  assert( (j * 4) == i);
  
}
