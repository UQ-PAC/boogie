procedure hbla12()
{
  var t: int;
  var s: int;
  var a: int;
  var b: int;
  var flag: bool;
  var x: int;
  var y: int;

  t := 0;
  s := 0;
  a := 0;
  b := 0;
  while(*){
    a := a + 1;
    b := b + 1;
    s := s + a;
    t := t + b;

    if(flag){
      t := t + a;
    }
  } 
  //2s >= t
  x := 1;
  if(flag){
    x := t - (2 * s) + 2;
  }
  //x <= 2
  y := 0;
  while(y<=x){
    if(*){
      y := y + 1;
    }
    else{
      y := y + 2;
    }
  }
  assert(y<=4);
}