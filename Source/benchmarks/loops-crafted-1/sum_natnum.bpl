procedure main() {
  var i: int;
  var sum: int;
  var SIZE: int;


  SIZE := 40000;
  i := 0;
  sum := 0; 
  while(i< SIZE){ 
      i := i + 1; 
      sum := sum + i;
  }
  assert( sum == ((SIZE *(SIZE+1)) div 2));
  
}
