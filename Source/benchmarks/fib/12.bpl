procedure fib12()
{
  var t: int;
  var s: int;
  var a: int;
  var b: int;
  var flag: int;
  var x: int;
  var y: int;

  var turn: int;



  assume(t == 0);
  assume(s == 0);
  assume(a == 0);
  assume(b == 0);
  assume(turn == 0);


  while(turn != 4){
    if(turn == 0){
      
      if(*){
        turn := 1;
      }
      else{
        turn := 2;
      }
    }
    if(turn == 1){
      a := a + 1;
      b := b + 1;
      s := s + a;
      t := t + b;

      if(flag > 0){
        t := t + a;
      }
      
      if(*){
        turn := 1;
      }
      else{
        turn := 2;
      }
    }
    else{
      if(turn == 2){
        x := 1;

        if(flag > 0){
          x := t - (2 * s) + 2;
        }
        y := 0;
        turn := 3;
      }
    }

    if(turn == 3){
      if(y <= x){
        
        if(*){
          y := y + 1;
        }
        else{
          y := y + 2;
        }
      }
      else{
        turn := 4;
      }
    }
  }

  assert(y <= 4);	
}