procedure unknown() returns (u: bool);

procedure fib46()
{
  var u: bool;
  var z: int;
  var x: int;
  var y: int;
  var w: int;

  assume(w == 1);
  assume(z == 0);
  assume(x == 0);
  assume(y == 0);

  //call u := unknown();
  while(u){
    if(w mod 2 == 1){
      x := x + 1;
      w := w + 1;
    }
    if(z mod 2 == 0){
      y := y + 1;
      z := z + 1;
    }

    w := w + 2;
    //call u := unknown();
  }

  assert(x <= 1);
}