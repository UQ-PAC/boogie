procedure main() {
  var n: int;
  var v: int;
  var s: int;
  var i: int;
  assume (n > 0 && n <= 255);
  v := 0;
  s := 0;
  i := 0;
  while (i < n) {
    havoc v;
    assume (v >= 0 && v <= 255);
    s := s + v;
    i := i + 1;
  }
  assert !(s < v);
  assert !(s > 65025);
}