procedure main()
{
  var len: int;
  var i: int;
  var j: int;

  var bufsize: int;
  var limit: int;
  assume (bufsize >= 0);
  limit := bufsize - 4;

  i := 0;
  while (i < len) {
    j := 0;
    while (i < len && j < limit) {
      if (i + 1 < len){ 
        assert(i+1<len);
        assert(0<=i);
        if(*) {
          assert(i<len);
          assert(0<=i);
          assert(j<bufsize);
          assert(0<=j);

          j := j + 1;
          i := i + 1;
          assert(i<len);
          assert(0<=i);
          assert(j<bufsize);
          assert(0<=j);

          j := j + 1;
          i := i + 1;
          assert(j<bufsize);
          assert(0<=j);
          j := j + 1;
        }
      } else {
        assert(i<len);
        assert(0<=i);
        assert(j<bufsize);
        assert(0<=j);
        j := j + 1;
        i := i + 1;
      }
    }
  }
}
