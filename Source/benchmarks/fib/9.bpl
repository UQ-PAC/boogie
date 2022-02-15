procedure fib9()
{
  var i: int;
  var pvlen: int;
  var t: int;
  var k: int;
  var n: int;
  var j: int;
  var turn: int;

  assume(k == 0);
  assume(i == 0);
  assume(turn == 0);

  while(*) {
    if (turn == 0) {
      i := i + 1;
      
      if (*) {
        turn := 0;
      } else {
        turn := 1;
      }
    } else {
      if (turn == 1) {
        if (i > pvlen) {
          pvlen := i;
        }
        i := 0;
        turn := 2;
      }
    }
    if (turn == 2) {
      t := i;
      i := i + 1;
      k := k + 1;
      
      if (*) {
        turn := 2;
      } else {
        turn := 3;
      }
    } else if (turn == 3) {
      
      if (*) {
        turn := 3;
      } else {
        turn := 4;
      }
    } else if (turn == 4) {
      j := 0;
      n := i;
      turn := 5;
    }
    
  }
  assert(k >= 0);
}