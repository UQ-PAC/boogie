procedure main() {
  var SIZE: int;
  var n: int;
  var i: int;
  var k: int;
  var j: int;

  SIZE := 20000001;
  assume(n >= 0 && n <= SIZE);
  assume(k >= 0);
  i := 0;
  while( i < n ) {
    i := i + 1;
  }
  j := i;
  while( j < n ) {
    j := j+1;
  }
  k := j;
  while( k < n ) {
    k := k+1;
  }
  assert((i+j+k) div 3 <= SIZE);
}
