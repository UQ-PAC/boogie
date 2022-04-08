procedure fib12()
{
  var t: int;
  var s: int;
  var a: int;
  var b: int;
  var flag: bool;
  var x: int;
  var y: int;

  var turn: int;

  var loop1done: bool;
  var loop2done: bool;



  assume(t == 0);
  assume(s == 0);
  assume(a == 0);
  assume(b == 0);
  assume(turn == 0);

  loop2done := false;
  if (*) {
    assert(2 * s >= t);
    loop1done := true;
    x := 1;

    if(flag){
      x := t - (2 * s) + 2;
    }
    assert(x <= 2);
    y := 0;
  } else {
    loop1done := false;
  }


  while(!loop2done){
    if(!loop1done){
      a := a + 1;
      b := b + 1;
      s := s + a;
      t := t + b;

      if(flag){
        t := t + a;
      }
      
      if (*) {
        assert(2 * s >= t);
        loop1done := true;
        x := 1;

        if(flag){
          x := t - (2 * s) + 2;
        }
        assert(x <= 2);
        y := 0;
      } else {
        loop1done := false;
      }
    } else {
      if(y <= x){
        if(*){
          y := y + 1;
        }
        else{
          y := y + 2;
        }
      }
      else{
        loop2done := true;
      }
    }
  }

  assert(y <= 4);	
}