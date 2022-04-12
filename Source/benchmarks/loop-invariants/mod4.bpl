// https://gitlab.com/sosy-lab/benchmarking/sv-benchmarks/-/blob/main/c/loop-invariants/mod4.c
procedure main() {
  var x: int;
  x := 0;
  while(*) {
    x := x + 4;
  }
  assert((x mod 4) == 0);
  
}
