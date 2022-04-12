procedure main() { 
  var i: int, sn: int;
  sn := 0;
  i := 1;
  while(i<=8) {
    sn := sn + 2;
    i := i + 1;
  }
  assert(sn==8*2 || sn == 0);
}

