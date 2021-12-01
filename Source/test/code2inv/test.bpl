procedure main()
{
  var n:int;
  var x:int;
  var y:int;
  assume(n >= 0);
  x := n;
  y := 0;
  while(x>0)
  {
    x := x - 1;
    y := y + 1;
  }
  assert(y==n);
}
