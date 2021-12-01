procedure fib9();



implementation fib9()
{
  var i: int;
  var pvlen: int;
  var t: int;
  var k: int;
  var n: int;
  var j: int;
  var turn: int;
  var u: bool;

  anon0:
    assume k == 0;
    assume i == 0;
    assume turn == 0;
    goto anon18_LoopHead;

  anon18_LoopHead:
    goto anon18_LoopDone, anon18_LoopBody;

  anon18_LoopBody:
    assume {:partition} u;
    goto anon19_Then, anon19_Else;

  anon19_Else:
    assume {:partition} turn != 0;
    goto anon21_Then, anon21_Else;

  anon21_Else:
    assume {:partition} turn != 1;
    goto anon9;

  anon9:
    goto anon23_Then, anon23_Else;

  anon23_Else:
    assume {:partition} turn != 2;
    goto anon25_Then, anon25_Else;

  anon25_Else:
    assume {:partition} turn != 3;
    goto anon27_Then, anon27_Else;

  anon27_Else:
    assume {:partition} turn != 4;
    goto anon18_LoopHead;

  anon27_Then:
    assume {:partition} turn == 4;
    j := 0;
    n := i;
    turn := 5;
    goto anon18_LoopHead;

  anon25_Then:
    assume {:partition} turn == 3;
    goto anon26_Then, anon26_Else;

  anon26_Else:
    turn := 4;
    goto anon18_LoopHead;

  anon26_Then:
    turn := 3;
    goto anon18_LoopHead;

  anon23_Then:
    assume {:partition} turn == 2;
    t := i;
    i := i + 1;
    k := k + 1;
    goto anon24_Then, anon24_Else;

  anon24_Else:
    turn := 3;
    goto anon18_LoopHead;

  anon24_Then:
    turn := 2;
    goto anon18_LoopHead;

  anon21_Then:
    assume {:partition} turn == 1;
    goto anon22_Then, anon22_Else;

  anon22_Else:
    assume {:partition} pvlen >= i;
    goto anon8;

  anon8:
    i := 0;
    turn := 2;
    goto anon9;

  anon22_Then:
    assume {:partition} i > pvlen;
    pvlen := i;
    goto anon8;

  anon19_Then:
    assume {:partition} turn == 0;
    i := i + 1;
    goto anon20_Then, anon20_Else;

  anon20_Else:
    turn := 1;
    goto anon9;

  anon20_Then:
    turn := 0;
    goto anon9;

  anon18_LoopDone:
    assume {:partition} !u;
    assert k >= 0;
    return;
}