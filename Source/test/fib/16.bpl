procedure fib16()
{
  var x: int;
  var y: int;
  var i: int;
  var j: int;

  assume(x == i);
  assume(y == j);

  while(x != 0){
    x := x - 1;
    y := y - 1;
  }

  assert((i != j) || (y == 0));	
}