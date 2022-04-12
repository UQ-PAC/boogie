procedure main() {
  var n: int;
  var i: int;
  var k: int;
  var j: int;

  //assume(n <= 1000000);
  assume (n >= 0);
  k := n;
  i := 0;
  while( i < n ) {
    k := k - 1;
    i := i + 2;
  }

  j := 0;
 
  while( j < n div 2 ) {
    assert(k > 0);
    k := k - 1;
    j := j + 1;
  }
}
