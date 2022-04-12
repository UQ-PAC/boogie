procedure f(z: int) returns(y: int)
ensures y == z + 2;
{
  y := z + 2;
}

procedure main() {
  var x: int;
  x := 0;

  while (x < 268435455) {
    call x := f(x);
  }

  assert((x mod 2) == 0);
}
