procedure main()
{
  var i: int, j: int, k: int;
  i := 0;
  j := 0;
  k := 0;

  while (k < 268435455) {
    i := i + 1;
    j := j + 2;
    k := k + 3;
    assert((k == 3*i) && (j == 2*i));
  }

}
