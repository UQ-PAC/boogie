procedure fib15()
{
  var i: int;
  var n: int;
  var j: int;
  var k: int;

  assume(n > 0);
  assume(k > n);
  assume(j == 0);

  while(j < n){
    j := j + 1;
    k := k - 1;
  }

  assert(k >= 0);
}