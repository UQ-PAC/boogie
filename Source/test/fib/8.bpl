procedure fib8()
{
  var x: int;
  var y: int;
  var u: bool;

  anon0:
    assume x == 0;
    assume y == 0;
    goto anon10_LoopHead;

  anon10_LoopHead:
    goto anon10_LoopDone, anon10_LoopBody;

  anon10_LoopBody:
    assume {:partition} u;
    goto anon11_Then, anon11_Else;

  anon11_Else:
    goto anon12_Then, anon12_Else;

  anon12_Else:
    goto anon8;

  anon8:
    goto anon10_LoopHead;

  anon12_Then:
    goto anon13_Then, anon13_Else;

  anon13_Else:
    assume {:partition} 4 > x;
    goto anon6;

  anon6:
    goto anon14_Then, anon14_Else;

  anon14_Else:
    assume {:partition} 0 <= x;
    goto anon8;

  anon14_Then:
    assume {:partition} x < 0;
    y := y + 1;
    goto anon8;

  anon13_Then:
    assume {:partition} x >= 4;
    x := x + 1;
    y := y + 1;
    goto anon6;

  anon11_Then:
    x := x + 1;
    y := y + 1;
    goto anon8;

  anon10_LoopDone:
    assume {:partition} !u;
    assert x < 4 || y > 2;
    return;
}
