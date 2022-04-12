procedure main() {
  var i: int;
  var x: int;
  var y: int;
  var n: int;
  assume (n > 0);
  x := 0;
  y := 0;
  i := 0;
  while (true) {
    assert(x == 0);
    i := i + 1;
  }
  assert (x != 0);
}