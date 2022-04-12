// adapted, original had end assertion guaranteed to be false
procedure main() {
  var i: int;
  var x: int;
  var y: int;
  var n: int;
  assume (n > 0);
  x := 0;
  y := 0;
  i := 0;
  while (i < n) {
    x := x - y;
    assert (x == 0);
    havoc y;
    assume (y != 0);
    x := x + y;
  }
  assert (x != 0);
}