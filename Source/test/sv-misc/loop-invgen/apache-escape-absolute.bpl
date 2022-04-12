procedure main()
{
  var scheme: int;
  var urilen: int;
  var tokenlen: int;
  var cp: int;
  var c: int;
  assume(urilen <= 1000000 && urilen >= -1000000);
  assume(tokenlen <= 1000000 && tokenlen >= -1000000);
  assume(scheme <= 1000000 && scheme >= -1000000);

  assume(urilen>0);
  assume(tokenlen>0);
  assume(scheme >= 0 );
  assume !(scheme == 0 || (urilen-1 < scheme));

  cp := scheme;

  assert(cp-1 < urilen);
  assert(0 <= cp-1);

  if (*) {
    assert(cp < urilen);
    assert(0 <= cp);
    while ( cp != urilen-1) {
      if(*) {
        break;
      }
      assert(cp < urilen);
      assert(0 <= cp);
      cp := cp + 1;
    }
    assert(cp < urilen);
    assert( 0 <= cp );
    assume !(cp == urilen-1);
    assert(cp+1 < urilen);
    assert( 0 <= cp+1 );
    assume !(cp+1 == urilen-1);
    cp := cp + 1;

    scheme := cp;

    if (*) {
      c := 0;
      assert(cp < urilen);
      assert(0<=cp);
      while ( cp != urilen-1 && c < tokenlen - 1) {
        assert(cp < urilen);
        assert(0<=cp);
        if (*) {
          c := c + 1;
          assert(c < tokenlen);
          assert(0<=c);
          assert(cp < urilen);
          assert(0<=cp);
        }
        cp := cp + 1;
      }
    }
  }
}
