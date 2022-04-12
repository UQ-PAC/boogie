procedure main ()
{
  var in: int;
  var inlen: int;
  var bufferlen: int;
  var buf: int;
  var buflim: int;

  assume(bufferlen >1);
  assume(inlen > 0);
  assume(bufferlen < inlen);

  buf := 0;
  in := 0;
  buflim := bufferlen - 2;

  while(*)
  {
    if (buf == buflim) {
      break;
    }
    assert(0<=buf);
    assert(buf<bufferlen);
  
    buf := buf + 1;
    in := in + 1;
    assert(0<=in);
    assert(in<inlen);
  }

  assert(0<=buf);
  assert(buf<bufferlen);
  buf := buf + 1;

  /* OK */
  assert(0<=buf);
 //6
  assert(buf<bufferlen);

  buf := buf + 1;

}
