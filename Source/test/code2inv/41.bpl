procedure main()
{
  var c: int;
  var n: int;
  var u: bool;

  anon0:
    c := 0;
    assume n > 0;
    goto anon8_LoopHead;

  anon8_LoopHead:
    goto anon8_LoopDone, anon8_LoopBody;

  anon8_LoopBody:
    assume {:partition} u;
    goto anon9_Then, anon9_Else;

  anon9_Else:
    goto anon11_Then, anon11_Else;

  anon11_Else:
    assume {:partition} c != n;
    goto anon6;

  anon6:
    havoc u;
    goto anon8_LoopHead;

  anon11_Then:
    assume {:partition} c == n;
    c := 1;
    goto anon6;

  anon9_Then:
    goto anon10_Then, anon10_Else;

  anon10_Else:
    assume {:partition} n >= c;
    goto anon6;

  anon10_Then:
    assume {:partition} c > n;
    c := c + 1;
    goto anon6;

  anon8_LoopDone:
    assume {:partition} !u;
    assert c != n ==> c <= n;
    return;
}