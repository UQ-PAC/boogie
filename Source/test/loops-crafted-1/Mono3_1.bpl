procedure main() {
  var x: int;
  var y: int;
  x := 0;
  y := 0;
  while (x < 1000000) {
    if (x<500000) {
	y := y + 1;
    } else {
	y := y - 1;
    }
	x := x + 1;
  }
  assert(y!=0) ;
}
