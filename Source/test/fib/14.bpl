procedure unknown() returns (u: bool);

procedure fib14()
{
  var u: bool;
  var a: int;
  var j: int;
  var m: int;

  assume(a == 0);
  assume(m > 0);
  assume(j == 1);

  while(j <= m){
    call u := unknown();
    if(u){
      a := a + 1;
    }
    else{
      a := a - 1;
    }

    j := j + 1;
  }

  assert(a >= -m);
  assert(a <= m);
}