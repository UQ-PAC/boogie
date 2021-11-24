procedure fib11()
{
  var x: int;
  var i: int;
  var j: int;
  
  assume(j == 0);
  assume(x > 0);
  assume(i == 0);

  while(i < x) {
      j := j + 2;
      i := i + 1;
  }
  assert(j == 2*x);
}