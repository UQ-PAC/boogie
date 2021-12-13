procedure main() {
  var x: int;
  x := 0;

  while (x < 100000000) {
    if (x < 10000000) {
      x := x + 1;
    } else {
      x := x + 2;
    }
  }

  assert((x mod 2)==0) ;
  
}
