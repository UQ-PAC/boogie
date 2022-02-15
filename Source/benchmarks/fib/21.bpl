

procedure fib21()
{

  var i: int;
  var j: int;
  var k: int;
  var c1: int;
  var c2: int;
  var n: int;
  var v: int;

  assume(n > 0 && n < 10);
  assume(k == 0);
  assume(i == 0);
  assume(c1 == 4000);
  assume(c2 == 2000);

  while(i < n){
    i := i + 1;
    
    
    if(*){
      v := 0;		
    }
    else{
      v := 1;
    }

    if(v == 0){
      k := k + c1;
    }
    else{
      k := k + c2;
    }
  }

  assert(k > n);	
}