procedure test() {
  var x: int, n: int, y: int;
  var q: int;
  assume (n >= 0);
  x := -1;
  x := increment2(x);
  y := 0;
  while (x != n) {
      x := x + 1;
      y := increment(y);
  }
  assert (y >= n);
}

function increment(a:int) returns(int);
axiom (forall i: int :: (increment(i) == i + 1));

function increment2(a:int) returns(int) {a + 1}