procedure main()
{
  var x: int;
  var n: int;

  anon0:
    x := 0;
    n := 10;
    goto LoopHead;

  LoopHead:
    assume {:partition} x < n;
    x := x + 1;
    goto LoopDone, LoopHead;

  LoopDone:
    assume {:partition} x >= n;
    assert (x == 10);
    return;
}