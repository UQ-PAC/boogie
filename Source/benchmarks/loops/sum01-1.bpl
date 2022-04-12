procedure main() { 
  var i: int, n: int, sn: int;
  sn := 0;
  i := 1;
  assume (n >= 0);
  while (i<=n) {
    if (i<10) {
      sn := sn + 2;
    }
  }
  assert(sn==n*2 || sn == 0);
}
