procedure main() {
  var u: bool;
  var s: int;
  s := 0;
  while(u) {
    if (s != 0) {
      s := s + 1;
    }
    havoc u;
    if (u) {
      assert(s == 0);
    }
    havoc u;
  }
  
}
