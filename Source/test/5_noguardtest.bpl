procedure fib5(flag: bool)
{
  var i: int;
  var j: int;
  var x: int;
  var y: int;

  anon0:
    i := 0;
    j := 0;
    x := 0;
    y := 0;
    goto anon5_LoopHead;

  anon5_LoopHead:
    goto anon5_LoopDone, anon5_LoopBody;

  anon5_LoopBody:
    x := x + 1;
    y := y + 1;
    i := i + x;
    j := j + y;
    goto anon6_Then, anon6_Else;

  anon6_Else:
    assume {:partition} !flag;
    goto anon3;

  anon3:
    goto anon5_LoopHead;

  anon6_Then:
    assume {:partition} flag;
    j := j + 1;
    goto anon3;

  anon5_LoopDone:
    assert j >= i;
    return;
}