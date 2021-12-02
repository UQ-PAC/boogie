procedure fib29()
{
  var u: bool;
  var a: int;
  var b: int;
  var c: int;
  var d: int;
  var x: int;
  var y: int;
  var turn: int;

  anon0:
    assume a == 1;
    assume b == 1;
    assume c == 2;
    assume d == 2;
    assume x == 3;
    assume y == 3;
    assume turn == 0;
    goto anon12_LoopHead;

  anon12_LoopHead:
    goto anon12_LoopDone, anon12_LoopBody;

  anon12_LoopBody:
    assume {:partition} u;
    goto anon13_Then, anon13_Else;

  anon13_Else:
    assume {:partition} turn != 0;
    goto anon15_Then, anon15_Else;

  anon15_Else:
    assume {:partition} turn != 1;
    goto anon10;

  anon10:
    havoc u;
    goto anon12_LoopHead;

  anon15_Then:
    assume {:partition} turn == 1;
    c := c - 1;
    b := b - 1;
    goto anon16_Then, anon16_Else;

  anon16_Else:
    turn := 0;
    goto anon10;

  anon16_Then:
    turn := 1;
    goto anon10;

  anon13_Then:
    assume {:partition} turn == 0;
    x := a + c;
    y := b + d;
    goto anon14_Then, anon14_Else;

  anon14_Else:
    assume {:partition} (x + y) mod 2 != 0;
    a := a - 1;
    goto anon5;

  anon5:
    turn := 1;
    goto anon10;

  anon14_Then:
    assume {:partition} (x + y) mod 2 == 0;
    a := a + 1;
    d := d + 1;
    goto anon5;

  anon12_LoopDone:
    assume {:partition} !u;
    assert a + c == b + d;
    return;
}