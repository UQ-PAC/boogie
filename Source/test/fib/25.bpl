procedure fib25()
{
  var u: bool;
  var x: int;
  var y: int;
  var i: int;
  var j: int;
  var turn: int;

  anon0:
    assume x == 0;
    assume y == 0;
    assume i == 0;
    assume j == 0;
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
    goto anon25_Then, anon25_Else;

  anon25_Else:
    assume {:partition} turn != 2;
    goto anon17;

  anon17:
    havoc u;
    goto anon19_LoopHead;

  anon25_Then:
    assume {:partition} turn == 2;
    goto anon26_Then, anon26_Else;

  anon26_Else:
    assume {:partition} j > i;
    y := y + 1;
    goto anon16;

  anon16:
    turn := 0;
    goto anon17;

  anon26_Then:
    assume {:partition} i >= j;
    x := x + 1;
    y := y + 1;
    goto anon16;

  anon22_Then:
    assume {:partition} turn == 1;
    goto anon23_Then, anon23_Else;

  anon23_Else:
    assume {:partition} x != y;
    j := j + 1;
    goto anon9;

  anon9:
    goto anon24_Then, anon24_Else;

  anon24_Else:
    turn := 2;
    goto anon17;

  anon24_Then:
    turn := 1;
    goto anon17;

  anon23_Then:
    assume {:partition} x == y;
    i := i + 1;
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
    assume {:partition} !u;
    assert i >= j;
    return;
}