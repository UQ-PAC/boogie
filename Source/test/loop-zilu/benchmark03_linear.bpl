procedure main() {
  var x: int;
  var y: int;
  var i: int;
  var j: int;
  var u: bool;
  var flag: bool;
  x := 0;
 y := 0;
  assume(i==0 && j==0);
  while(u) {
    x := x + 1;
    y := y + 1;
    i := i + x;
    j := j + y;
    if (flag) {
      j := j + 1;
    }
    havoc u;
  }
  assert(j>=i);
  
}
