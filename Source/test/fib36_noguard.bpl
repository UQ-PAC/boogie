procedure fib36()
{
  var a: int;
  var b: int;
  var x: int;
  var y: int;
  var z: int;
  var w: int;
  var flag: int;
  var i: int;
  var j: int;
  var k: int;
  var c: int;
  var d: int;
  var turn: int;

  anon0:
    assume a == 0;
    assume b == 0;
    assume x == 0;
    assume y == 0;
    assume z == 0;
    assume j == 0;
    assume w == 0;
    assume turn == 0;
    goto anon35_LoopHead;

  anon35_LoopHead:
    goto anon35_LoopDone, anon35_LoopBody;

  anon35_LoopBody:
    goto anon36_Then, anon36_Else;

  anon36_Else:
    assume {:partition} turn != 0;
    goto anon5;

  anon5:
    goto anon38_Then, anon38_Else;

  anon38_Else:
    assume {:partition} turn != 1;
    goto anon7;

  anon7:
    goto anon39_Then, anon39_Else;

  anon39_Else:
    assume {:partition} turn != 2;
    goto anon41_Then, anon41_Else;

  anon41_Else:
    assume {:partition} turn != 3;
    goto anon43_Then, anon43_Else;

  anon43_Else:
    assume {:partition} turn != 4;
    goto anon46_Then, anon46_Else;

  anon46_Else:
    assume {:partition} turn != 5;
    goto anon24;

  anon24:
    goto anon47_Then, anon47_Else;

  anon47_Else:
    assume {:partition} turn != 6;
    goto anon48_Then, anon48_Else;

  anon48_Else:
    assume {:partition} turn != 7;
    goto anon33;

  anon33:
    goto anon35_LoopHead;

  anon48_Then:
    assume {:partition} turn == 7;
    c := c + 1;
    d := d + 1;
    goto anon49_Then, anon49_Else;

  anon49_Else:
    assume {:partition} 0 >= flag;
    a := a + c;
    b := b + d;
    goto anon30;

  anon30:
    goto anon50_Then, anon50_Else;

  anon50_Else:
    turn := 0;
    goto anon33;

  anon50_Then:
    turn := 7;
    goto anon33;

  anon49_Then:
    assume {:partition} flag > 0;
    a := a + 1;
    b := b + 1;
    goto anon30;

  anon47_Then:
    assume {:partition} turn == 6;
    c := 0;
    d := 0;
    turn := 7;
    goto anon33;

  anon46_Then:
    assume {:partition} turn == 5;
    z := z + 1;
    w := x + y + 1;
    turn := 0;
    goto anon24;

  anon43_Then:
    assume {:partition} turn == 4;
    goto anon44_Then, anon44_Else;

  anon44_Else:
    assume {:partition} x mod 2 != 0;
    x := x - 1;
    y := y - 1;
    goto anon19;

  anon19:
    goto anon45_Then, anon45_Else;

  anon45_Else:
    turn := 5;
    goto anon24;

  anon45_Then:
    turn := 4;
    goto anon24;

  anon44_Then:
    assume {:partition} x mod 2 == 0;
    x := x + 2;
    y := y - 2;
    goto anon19;

  anon41_Then:
    assume {:partition} turn == 3;
    x := z;
    y := k;
    goto anon42_Then, anon42_Else;

  anon42_Else:
    assume {:partition} x mod 2 != 1;
    turn := 4;
    goto anon24;

  anon42_Then:
    assume {:partition} x mod 2 == 1;
    x := x + 1;
    y := y - 1;
    goto anon24;

  anon39_Then:
    assume {:partition} turn == 2;
    goto anon40_Then, anon40_Else;

  anon40_Else:
    assume {:partition} j <= i;
    turn := 3;
    goto anon24;

  anon40_Then:
    assume {:partition} i < j;
    k := k + 1;
    i := i + 1;
    goto anon24;

  anon38_Then:
    assume {:partition} turn == 1;
    i := z;
    j := w;
    k := 0;
    turn := 2;
    goto anon7;

  anon36_Then:
    assume {:partition} turn == 0;
    goto anon37_Then, anon37_Else;

  anon37_Else:
    turn := 6;
    goto anon5;

  anon37_Then:
    turn := 1;
    goto anon5;

  anon35_LoopDone:
    assert w >= z && a == b;
    return;
}