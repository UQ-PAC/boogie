procedure unknown() returns (u: bool);

procedure main() {
  // variable declarations
  var lock:int;
  var x:int;
  var y:int;
  var u:bool;
  // pre-conditions
  x := y;
  lock := 1;
  // loop body
  while ((x != y)) {
      havoc u;
      if (u) {
        lock  :=  1;
        x  :=  y;
      } else {
        lock  :=  0;
        x  :=  y;
        y  :=  (y + 1);
      }

  }
  // post-condition
assert( (lock == 1) );
}
