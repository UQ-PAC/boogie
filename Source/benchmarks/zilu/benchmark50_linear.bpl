procedure main() {
  var xa: int;
  var ya: int;
  assume(xa + ya > 0);
  while (xa > 0) {
    xa := xa - 1;
    ya := ya + 1;
  }
  assert(ya >= 0);
  
}
