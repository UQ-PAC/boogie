procedure hola31()
{
  var i: int;
  var j: int;
  var k: int;
  var n: int;
  var m: int;
  var block: int;

  anon0:
    block := 0;
    assume m + 1 < n;
    i := 0;
    goto anon13_LoopHead;

  anon13_LoopHead:
    goto anon13_LoopDone, anon13_LoopBody;

  anon13_LoopBody:
    assume  block != -1;
    goto anon14_Then, anon14_Else;

  anon14_Else:
    assume  block != 0;
    goto anon16_Then, anon16_Else;

  anon16_Else:
    assume  block != 1;
    goto anon19_Then, anon19_Else;

  anon19_Else:
    assume  block != 2;
    goto anon13_LoopHead;

  anon19_Then:
    assume  block == 2;
    goto anon20_Then, anon20_Else;

  anon20_Else:
    assume  j <= k;
    block := 1;
    goto anon13_LoopHead;

  anon20_Then:
    assume  k < j;
    k := k + 1;
    goto anon13_LoopHead;

  anon16_Then:
    assume  block == 1;
    goto anon17_Then, anon17_Else;

  anon17_Else:
    assume  m <= j;
    i := i + 4;
    block := 0;
    goto anon13_LoopHead;

  anon17_Then:
    assume  j < m;
    goto anon18_Then, anon18_Else;

  anon18_Else:
    assert n + j + 5 > i;
    j := j + 2;
    goto anon13_LoopHead;

  anon18_Then:
    assert j >= 0;
    j := j + 1;
    k := 0;
    block := 2;
    goto anon13_LoopHead;

  anon14_Then:
    assume  block == 0;
    goto anon15_Then, anon15_Else;

  anon15_Else:
    assume  n <= i;
    block := -1;
    goto anon13_LoopHead;

  anon15_Then:
    assume  i < n;
    j := i;
    block := 1;
    goto anon13_LoopHead;

  anon13_LoopDone:
    assume  block == -1;
    return;
}