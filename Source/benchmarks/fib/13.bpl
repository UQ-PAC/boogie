procedure fib13()
{

  var j: int;
  var k: int;
  var t: int;

  assume(j == 2);
  assume(k == 0);

  while(*){
    if(t == 0) {
      j := j + 4;
    } else {
      j := j + 2;
      k := k + 1;
    }
    
  }

  assert(k == 0 || j == (2*k) + 2);	
}
