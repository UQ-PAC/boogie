procedure test() {
  var x: int, n: int where n >= 0, y: int;
  var q: int;
  havoc n;
  assume (x == 0);
  assume (y == 0);
  while (x != n) {
      x := x + 1;
      //y := y + 1;
      call y := increment(y);
  }
  assert (y == n);
}


//function increment(a:int) returns(int) { a + 1 }


procedure increment(a: int) returns (b: int);
  ensures b == a + 1;
