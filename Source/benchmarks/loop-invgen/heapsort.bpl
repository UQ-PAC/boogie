procedure main(){
  var n: int;
  var l: int;
  var r: int;
  var i: int;
  var j: int;
  assume(1 <= n);


  l := (n div 2) + 1;
  r := n;
  if(l>1) {
    l := l - 1;
  } else {
    r := r - 1;
  }
  while(r > 1) {
    i := l;
    j := 2*l;
    while(j <= r) {
      if( j < r) {
        assert(1 <= j);
        assert(j <= n);
        assert(1 <= j+1);
        assert(j+1 <= n);
        if(*) {
          j := j + 1;
        }
      }
      assert(1 <= j);
      assert(j <= n);
      if(*) { 
      	break;
      }
      assert(1 <= i);
      assert(i <= n);
      assert(1 <= j);
      assert(j <= n);
      i := j;
      j := 2*j;
    }
    if(l > 1) {
      assert(1 <= l);
      assert(l <= n);
      l := l - 1;
    } else {
      assert(1 <= r);
      assert(r <= n);
      r := r - 1;
    }
  }
}
