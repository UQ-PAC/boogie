procedure main() {

  var c1: int;
  var c2: int;
  var c3: int;
  var n: int;
  var v: int;
  var i: int;
  var k: int;
  var j: int;

  c1 := 4000;
  c2 := 2000;
  c3 := 10000;

  assume(0 <= n && n < 10);

  k := 0;
  i := 0;
  while( i < n ) {
    i := i + 1;
    havoc v;
    assume(0 <= v && n < 2);
    if( v == 0 ) {
      k := k + c1;
    } else {
      if( v == 1 ) {
        k := k + c2;
      } else {
        k := k + c3;
      }
    } 
  }

  j := 0;
  while( j < n ) {
    assert(k > 0);
    j := j + 1;
    k := k + 1;
  }
}
