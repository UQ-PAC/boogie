procedure test() {
  var x: int, n: int where n >= 0, y: int;
  var q: int;
  havoc n;
  x := -1;
  x := increment(x);
  y := 0;
  while (x != n) {
      x := x + 1;
      //y := y + 1;
      y := increment(y);
  }
  assert (y >= n);
}

function increment(a:int) returns(int) {a + 1}
//axiom (forall i: int :: {increment(i)} (increment(i) == i + 1));