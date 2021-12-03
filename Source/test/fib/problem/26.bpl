procedure fib26()
{
  var u: bool;
  var w: int;
  var z: int;
  var x: int;
  var y: int;
  var turn: int;

  anon0:
    assume w == 1;
    assume z == 0;
    assume x == 0;
    assume y == 0;
    assume turn == 0;
    goto anon19_LoopHead;

  anon19_LoopHead:
    goto anon19_LoopDone, anon19_LoopBody;

  anon19_LoopBody:
    assume {:partition} u;
    goto anon20_Then, anon20_Else;

  anon20_Else:
    assume {:partition} turn != 0;
    goto anon5;

  anon5:
    goto anon22_Then, anon22_Else;

  anon22_Else:
    assume {:partition} turn != 1;
    goto anon13;

  anon13:
    goto anon26_Then, anon26_Else;

  anon26_Else:
    assume {:partition} turn != 2;
    goto anon17;

  anon17:
    havoc u;
    goto anon19_LoopHead;

  anon26_Then:
    assume {:partition} turn == 2;
    z := x + y;
    w := z + 1;
    goto anon27_Then, anon27_Else;

  anon27_Else:
    turn := 0;
    goto anon17;

  anon27_Then:
    turn := 2;
    goto anon17;

  anon22_Then:
    assume {:partition} turn == 1;
    goto anon23_Then, anon23_Else;

  anon23_Else:
    assume {:partition} w mod 2 != 1;
    goto anon8;

  anon8:
    goto anon24_Then, anon24_Else;

  anon24_Else:
    assume {:partition} z mod 2 != 0;
    goto anon10;

  anon10:
    goto anon25_Then, anon25_Else;

  anon25_Else:
    turn := 2;
    goto anon13;

  anon25_Then:
    turn := 1;
    goto anon13;

  anon24_Then:
    assume {:partition} z mod 2 == 0;
    y := y + 1;
    goto anon10;

  anon23_Then:
    assume {:partition} w mod 2 == 1;
    x := x + 1;
    goto anon8;

  anon20_Then:
    assume {:partition} turn == 0;
    goto anon21_Then, anon21_Else;

  anon21_Else:
    turn := 2;
    goto anon5;

  anon21_Then:
    turn := 1;
    goto anon5;

  anon19_LoopDone:
    assume {:partition} !u;
    assert x == y;
    return;
}