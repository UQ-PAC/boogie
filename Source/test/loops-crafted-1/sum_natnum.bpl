procedure main() {
  var i: int;
  var sum: int;
  i := 0;
  sum := 0; 
  while(i< 40000){ 
      i := i + 1; 
      sum := sum + i;
  }
  assert( sum == ((40000 *(40000+1)) div 2));
  
}
