procedure main()
{
  /* buf is treated as an array of unsigned 8-byte ints */
  //  u_int8_t *p;
  var p: int;
  var i: int;
  var leader_len: int;
  var bufsize: int;
  var bufsize_0: int;
  var ielen: int;

  assume(leader_len < 1000000);
  assume(bufsize < 1000000);
  assume(ielen < 1000000);

  // copy the contents of leader into buf
  assume (leader_len >0);
  assume (bufsize >0);
  assume (ielen >0); 

  assume !(bufsize < leader_len);

  p := 0;

  bufsize_0 := bufsize;
  bufsize := bufsize - leader_len;
  p := p + leader_len;

  /* This is the fix. */
  assume !(bufsize < 2*ielen);

  i := 0;
  while (i < ielen && bufsize > 2) {
    assert(0<=p);
    assert(p+1<bufsize_0);
    p := p + 2;
    i := i + 1;
  } 
}

