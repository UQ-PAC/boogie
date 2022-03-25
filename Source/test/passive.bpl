after conversion to passive commands
implementation fib30()
{
  var i: int;
  var c: int;
  var n: int;
  var c#AT#0: int;
  var i#AT#0: int;
  var c#AT#1: int;
  var i#AT#1: int;

  PreconditionGeneratedEntry:
    goto 0;

  anon0:
    assume i == 0;
    assume c == 0;
    assume n > 0;
    assert {:inferred} !(c <= -1 || i <= -1);
    goto anon3_LoopHead;

  anon3_LoopHead:  // cut point
    assume !(c#AT#0 <= -1 || i#AT#0 <= -1);
    goto anon3_LoopDone, anon3_LoopBody;

  anon3_LoopBody:
    assume {:partition} i#AT#0 < n;
    assume c#AT#1 == c#AT#0 + i#AT#0;
    assume i#AT#1 == i#AT#0 + 1;
    assert {:inferred} !(c#AT#1 <= -1 || i#AT#1 <= -1);
    assume false;
    goto GeneratedUnifiedExit;

  anon3_LoopDone:
    assume {:partition} n <= i#AT#0;
    assert c#AT#0 >= 0;
    goto GeneratedUnifiedExit;

  GeneratedUnifiedExit:
    return;

  0:
    goto anon0;
}