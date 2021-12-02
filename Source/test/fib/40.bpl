procedure fib40()
{
  var i: int;
  var j: int;
  var flag: int;
  var a: int;
  var b: int;
  var turn: int;

  anon0:
    assume j == 1;
    assume (flag > 0 && i == 0) || (flag <= 0 && i == 1);
    assume turn == 0;
    goto anon19_LoopHead;

  anon19_LoopHead:
    goto anon19_LoopDone, anon19_LoopBody;

  anon19_LoopBody:
    assume {:partition} turn != 4;
    goto anon20_Then, anon20_Else;

  anon20_Else:
    assume {:partition} turn != 0;
    goto anon5;

  anon5:
    goto anon22_Then, anon22_Else;

  anon22_Else:
    assume {:partition} turn != 1;
    goto anon25_Then, anon25_Else;

  anon25_Else:
    assume {:partition} turn != 2;
    goto anon26_Then, anon26_Else;

  anon26_Else:
    assume {:partition} turn != 3;
    goto anon19_LoopHead;

  anon26_Then:
    assume {:partition} turn == 3;
    a := a + 1;
    b := b + j - i;
    goto anon27_Then, anon27_Else;

  anon27_Else:
    turn := 4;
    goto anon19_LoopHead;

  anon27_Then:
    turn := 3;
    goto anon19_LoopHead;

  anon25_Then:
    assume {:partition} turn == 2;
    a := 0;
    b := 0;
    turn := 3;
    goto anon19_LoopHead;

  anon22_Then:
    assume {:partition} turn == 1;
    i := i + 2;
    goto anon23_Then, anon23_Else;

  anon23_Else:
    assume {:partition} i mod 2 != 0;
    j := j + 1;
    goto anon9;

  anon9:
    goto anon24_Then, anon24_Else;

  anon24_Else:
    turn := 2;
    goto anon19_LoopHead;

  anon24_Then:
    turn := 1;
    goto anon19_LoopHead;

  anon23_Then:
    assume {:partition} i mod 2 == 0;
    j := j + 2;
    goto anon9;

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
    assume {:partition} turn == 4;
    assert flag <= 0 || a == b;
    return;
}