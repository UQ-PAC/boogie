procedure fib12();



implementation fib12()
{
  var t: int;
  var s: int;
  var a: int;
  var b: int;
  var flag: int;
  var x: int;
  var y: int;
  var turn: int;

  anon0:
    assume t == 0;
    assume s == 0;
    assume a == 0;
    assume b == 0;
    assume turn == 0;
    goto anon22_LoopHead;

  anon22_LoopHead:
    goto anon22_LoopDone, anon22_LoopBody;

  anon22_LoopBody:
    assume {:partition} turn != 4;
    goto anon23_Then, anon23_Else;

  anon23_Else:
    assume {:partition} turn != 0;
    goto anon5;

  anon5:
    goto anon25_Then, anon25_Else;

  anon25_Else:
    assume {:partition} turn != 1;
    goto anon28_Then, anon28_Else;

  anon28_Else:
    assume {:partition} turn != 2;
    goto anon15;

  anon15:
    goto anon30_Then, anon30_Else;

  anon30_Else:
    assume {:partition} turn != 3;
    goto anon22_LoopHead;

  anon30_Then:
    assume {:partition} turn == 3;
    goto anon31_Then, anon31_Else;

  anon31_Else:
    assume {:partition} x < y;
    turn := 4;
    goto anon22_LoopHead;

  anon31_Then:
    assume {:partition} y <= x;
    goto anon32_Then, anon32_Else;

  anon32_Else:
    y := y + 2;
    goto anon22_LoopHead;

  anon32_Then:
    y := y + 1;
    goto anon22_LoopHead;

  anon28_Then:
    assume {:partition} turn == 2;
    x := 1;
    goto anon29_Then, anon29_Else;

  anon29_Else:
    assume {:partition} 0 >= flag;
    goto anon14;

  anon14:
    y := 0;
    turn := 3;
    goto anon15;

  anon29_Then:
    assume {:partition} flag > 0;
    x := t - 2 * s + 2;
    goto anon14;

  anon25_Then:
    assume {:partition} turn == 1;
    a := a + 1;
    b := b + 1;
    s := s + a;
    t := t + b;
    goto anon26_Then, anon26_Else;

  anon26_Else:
    assume {:partition} 0 >= flag;
    goto anon8;

  anon8:
    goto anon27_Then, anon27_Else;

  anon27_Else:
    turn := 2;
    goto anon15;

  anon27_Then:
    turn := 1;
    goto anon15;

  anon26_Then:
    assume {:partition} flag > 0;
    t := t + a;
    goto anon8;

  anon23_Then:
    assume {:partition} turn == 0;
    goto anon24_Then, anon24_Else;

  anon24_Else:
    turn := 2;
    goto anon5;

  anon24_Then:
    turn := 1;
    goto anon5;

  anon22_LoopDone:
    assume {:partition} turn == 4;
    assert y <= 4;
    return;
}