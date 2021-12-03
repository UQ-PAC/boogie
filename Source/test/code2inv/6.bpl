
procedure main()
{
    var v1:int;
    var v2:int;
    var v3:int;
    var x:int; 
    var size:int;
    var y:int;
    var z:int;
    x := 0;

    while(x < size) {
       x := x + 1;
       if( z <= y) {
          y := z;
       }
    }

    assert ((size > 0) ==> z >= y);
}
