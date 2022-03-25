procedure main()
{
  var x: int;
  var y: int;
  var x_0: int;
  var x_1: int;
  var x_2: int;
  var y_1: int;
  var y_2: int;

  anon0:
    assume x_0 == 1;
    x := x_0;
    goto anon3_LoopHead;

  anon3_LoopHead:  // cut point
    assume y_1 == y && x_1 == x;
    goto anon3_LoopDone, anon3_LoopBody;

  anon3_LoopBody:
    assume {:partition} x_1 <= 10;
    assume y_2 == 10 - x_1;
    assume x_2 == x_1 + 1;
    x := x_2;
    y := y_2;
    goto anon3_LoopHead;

  anon3_LoopDone:
    assume {:partition} 10 < x_1;
    assert y_1 >= 0;
    goto GeneratedUnifiedExit;

  GeneratedUnifiedExit:
    return;
}