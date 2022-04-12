//https://gitlab.com/sosy-lab/benchmarking/sv-benchmarks/-/blob/main/c/loop-industry-pattern/nested-3.c
procedure main() {
  var last: int;
  var a: int;
  var b: int;
  var c: int;
  var st: int;
  a := 0;
  b := 0;
  c := 0;
  st := 0;
  while (true) {
    st := 1;
    c := 0;
    while (c < 200000) {
      if (c == last) {
        st := 0;
      }
    }
    if (st == 0 && c == last + 1) {
      a := a + 3;
      b := b + 3;
    } else {
      a := a + 2;
      b := b + 2;
    }
    if (c == last && st == 0) {
      a := a + 1;
    }
    assert (a == b && c == 200000);
  }
}