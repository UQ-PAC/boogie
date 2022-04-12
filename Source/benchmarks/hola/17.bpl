procedure hola17()
{
	var k: int;
	var i: int;
	var j: int;
	var n: int;

  k := 1;
  i := 1;
  j := 0;
  while (i < n) {
    j := 0;
    while (j < i) {
      k := k + (i - j);
      j := j + 1;
    }
    i := i + 1;
  }
  assert (k >= n);
}