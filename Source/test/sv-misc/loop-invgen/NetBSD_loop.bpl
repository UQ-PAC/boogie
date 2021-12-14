procedure main ()
{
  var MAXPATHLEN: int;
  var pathbuf_off: int;

  /* Char *bound := pathbuf + sizeof(pathbuf)/sizeof(*pathbuf) - 1;
 */
  var bound_off: int;

  /* glob2's local vars */
  /* Char *p;
 */
  var glob2_p_off: int;
  var glob2_pathbuf_off: int;
  var glob2_pathlim_off: int;

  assume(MAXPATHLEN > 0 && MAXPATHLEN < 2147483647);

  pathbuf_off := 0;
  bound_off := pathbuf_off + (MAXPATHLEN + 1) - 1;

  glob2_pathbuf_off := pathbuf_off;
  glob2_pathlim_off := bound_off;
  glob2_p_off := glob2_pathbuf_off;
  while (glob2_p_off <= glob2_pathlim_off) {
    /* OK */
    assert (0 <= glob2_p_off);
    assert (glob2_p_off < MAXPATHLEN + 1);
    glob2_p_off := glob2_p_off + 1;
  }

}
