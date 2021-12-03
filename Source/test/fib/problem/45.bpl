procedure fib45()
{
  var x: int;
  var y: int;
  var i: int;
  var j: int;
  var c: int;
  var d: int;
  var flag: int;
  var w: int;
  var z: int;
  var turn: int;

  anon0:
    assume x == 0;
    assume y == 0;
    assume i == 0;
    assume j == 0;
    assume c == 0;
    assume d == 1;
    assume turn == 0;
    goto anon31_LoopHead;

  anon31_LoopHead:
    goto anon31_LoopDone, anon31_LoopBody;

  anon31_LoopBody:
    assume {:partition} turn != 6;
    goto anon32_Then, anon32_Else;

  anon32_Else:
    assume {:partition} turn != 0;
    goto anon5;

  anon5:
    goto anon34_Then, anon34_Else;

  anon34_Else:
    assume {:partition} turn != 1;
    goto anon37_Then, anon37_Else;

  anon37_Else:
    assume {:partition} turn != 2;
    goto anon16;

  anon16:
    goto anon39_Then, anon39_Else;

  anon39_Else:
    assume {:partition} turn != 3;
    goto anon40_Then, anon40_Else;

  anon40_Else:
    assume {:partition} turn != 4;
    goto anon44_Then, anon44_Else;

  anon44_Else:
    assume {:partition} turn != 5;
    goto anon31_LoopHead;

  anon44_Then:
    assume {:partition} turn == 5;
    z := x + y;
    w := z + 1;
    goto anon45_Then, anon45_Else;

  anon45_Else:
    turn := 6;
    goto anon31_LoopHead;

  anon45_Then:
    turn := 3;
    goto anon31_LoopHead;

  anon40_Then:
    assume {:partition} turn == 4;
    goto anon41_Then, anon41_Else;

  anon41_Else:
    assume {:partition} w mod 2 != 1;
    goto anon21;

  anon21:
    goto anon42_Then, anon42_Else;

  anon42_Else:
    assume {:partition} z mod 2 != 0;
    goto anon23;

  anon23:
    goto anon43_Then, anon43_Else;

  anon43_Else:
    turn := 5;
    goto anon31_LoopHead;

  anon43_Then:
    turn := 4;
    goto anon31_LoopHead;

  anon42_Then:
    assume {:partition} z mod 2 == 0;
    y := y + 1;
    goto anon23;

  anon41_Then:
    assume {:partition} w mod 2 == 1;
    x := x + 1;
    goto anon21;

  anon39_Then:
    assume {:partition} turn == 3;
    turn := 4;
    goto anon31_LoopHead;

  anon37_Then:
    assume {:partition} turn == 2;
    goto anon38_Then, anon38_Else;

  anon38_Else:
    assume {:partition} i > j;
    x := y + 1;
    goto anon15;

  anon15:
    w := 1;
    z := 0;
    turn := 3;
    goto anon16;

  anon38_Then:
    assume {:partition} j >= i;
    x := y;
    goto anon15;

  anon34_Then:
    assume {:partition} turn == 1;
    x := x + 1;
    y := y + 1;
    i := i + x;
    j := j + y;
    goto anon35_Then, anon35_Else;

  anon35_Else:
    assume {:partition} 0 >= flag;
    goto anon8;

  anon8:
    goto anon36_Then, anon36_Else;

  anon36_Else:
    turn := 2;
    goto anon16;

  anon36_Then:
    turn := 1;
    goto anon16;

  anon35_Then:
    assume {:partition} flag > 0;
    j := j + 1;
    goto anon8;

  anon32_Then:
    assume {:partition} turn == 0;
    goto anon33_Then, anon33_Else;

  anon33_Else:
    turn := 2;
    goto anon5;

  anon33_Then:
    turn := 1;
    goto anon5;

  anon31_LoopDone:
    assume {:partition} turn == 6;
    assert x == y;
    return;
}