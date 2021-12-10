procedure main() {
  var i: int;
  var k: int;
  var u: bool;
  assume(0 <= k && k <= 1 && i == 1);
  while(u) {
    i := i + 1;
    k := k - 1;
    havoc u;
  }
  assert(1 <= i + k && i + k <= 2 && i >= 1);
  
}
