// https://gitlab.com/sosy-lab/benchmarking/sv-benchmarks/-/blob/main/c/loop-invgen/fragtest_simple.c
procedure hola9(){
  var i: int;
  var pvlen: int;
  var t: int;
  var k: int;
  var n: int;
  var j: int;
  k := 0;
  i := 0;

  //  pkt = pktq->tqh_first;
  while (*) {
    // if (i > 1000000) {
    //  break;
    //}
    i := i + 1;
  }
  if (i > pvlen) {
    pvlen := i;
  }
  i := 0;

  while (*) {
    //if (i > 1000000) {
    //  break;
    //}
    t := i;
    i := i + 1;
    k := k + 1;
  }

  j := 0;
  n := i;
  while (true) {
    assert(k >= 0);
    k := k -1;
    i := i - 1;
    j := j + 1;
    if (j >= n) {
      break;
    }
  }
}