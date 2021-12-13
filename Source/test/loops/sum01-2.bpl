procedure main() { 
  var i: int, n: int, sn: int;
  sn := 0;
  assume(n < 1000 && n >= -1000);
  i := 1;
  while(i<=n) {
    sn := sn + 2;
    i := i + 1;
  }
  assert(sn==n*2 || sn == 0);
}
