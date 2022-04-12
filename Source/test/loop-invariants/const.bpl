// https://gitlab.com/sosy-lab/benchmarking/sv-benchmarks/-/blob/main/c/loop-invariants/const.c
procedure main() {
  var s: int;
  s := 0;
  while(*) {
    if (s != 0) {
      s := s + 1;
    }
    if (*) {
      assert(s == 0);
    }
  }
  
}
