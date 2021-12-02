procedure fib9()
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
    goto anon19_LoopHead;

  anon19_LoopHead:
    goto anon19_LoopDone, anon19_LoopBody;

  anon19_LoopBody:
    assume {:partition} u;
    goto anon20_Then, anon20_Else;

  anon20_Else:
    assume {:partition} turn != 0;
    goto anon22_Then, anon22_Else;

  anon22_Else:
    assume {:partition} turn != 1;
    goto anon9;

  anon9:
    goto anon24_Then, anon24_Else;

  anon24_Else:
    assume {:partition} turn != 2;
    goto anon26_Then, anon26_Else;

  anon26_Else:
    assume {:partition} turn != 3;
    goto anon28_Then, anon28_Else;

  anon28_Else:
    assume {:partition} turn != 4;
    goto anon17;

  anon17:
    havoc u;
    goto anon19_LoopHead;

  anon28_Then:
    assume {:partition} turn == 4;
    j := 0;
    n := i;
    turn := 5;
    goto anon17;

  anon26_Then:
    assume {:partition} turn == 3;
    goto anon27_Then, anon27_Else;

  anon27_Else:
    turn := 4;
    goto anon17;

  anon27_Then:
    turn := 3;
    goto anon17;

  anon24_Then:
    assume {:partition} turn == 2;
    t := i;
    i := i + 1;
    k := k + 1;
    goto anon25_Then, anon25_Else;

  anon25_Else:
    turn := 3;
    goto anon17;

  anon25_Then:
    turn := 2;
    goto anon17;

  anon22_Then:
    assume {:partition} turn == 1;
    goto anon23_Then, anon23_Else;

  anon23_Else:
    assume {:partition} pvlen >= i;
    goto anon8;

  anon8:
    i := 0;
    turn := 2;
    goto anon9;

  anon23_Then:
    assume {:partition} i > pvlen;
    pvlen := i;
    goto anon8;

  anon20_Then:
    assume {:partition} turn == 0;
    i := i + 1;
    goto anon21_Then, anon21_Else;

  anon21_Else:
    turn := 1;
    goto anon9;

  anon21_Then:
    turn := 0;
    goto anon9;

  anon19_LoopDone:
    assume {:partition} !u;
    assert k >= 0;
    return;
}