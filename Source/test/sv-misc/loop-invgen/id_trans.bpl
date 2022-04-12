procedure main() {
  var idBitLength: int, material_length: int, nlen: int;
  var j: int, k: int;
  assume (idBitLength >= 0 && idBitLength <= 4294967295);
  assume (material_length >= 0 && material_length <= 4294967295);
  assume( nlen  ==  idBitLength div 32 );
  j := 0;
  while ((j < idBitLength div 8) && (j < material_length)) {
    assert( 0 <= j);
    assert( j < material_length );
    assert( 0 <= j div 4 );
    assert( j div 4 < nlen);
    j := j + 1;
  }
  
  
}
