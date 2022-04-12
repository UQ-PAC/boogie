procedure hola40()
{

	var i: int;
	var j: int;
	var k: int;
	var flag: int;

	var a: int;
	var b: int;

  j := 1;
  if (flag != 0) {
    i := 0;
  } else {
    i := 1;
  }

  while (*) {
    i := i + 2;

    if(i mod 2 == 0){
      j := j + 2;
    } else{
      j := j + 1;
    }
  }
  a := 0;
  b := 0;

  while (*) {
    a := a + 1;
    b := b + (j - i);
  }

  if (flag !=0) {
    assert(a==b);
  }
}