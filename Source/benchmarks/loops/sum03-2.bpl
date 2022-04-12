procedure main() { 
  var sn: int, x: int; 
  sn := 0;
  x := 0;
  while(true){
    sn := sn + 2;
    x := x + 1;
    assert(sn==x*2 || sn == 0);
  }
}

