procedure test() {
  var x: int, n: int where n >= 0, y: int;
  var q: int;
  havoc n;
  x := -1;
  x := increment2(x);
  y := 0;
  while (x != n) {
      x := x + 1;
      //y := y + 1;
      y := increment(y);
  }
  assert (y >= n);
}

function increment(a:int) returns(int);
axiom (forall i: int :: (increment(i) == i + 1));

function increment2(a:int) returns(int) {a + 1}