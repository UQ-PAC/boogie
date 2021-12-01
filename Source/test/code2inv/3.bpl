
procedure main()
{
    var x:int;
    var y:int;
    var z:int; 
    x := 0;

    while(x < 5) {
       x := x + 1;
       if( z <= y) {
          y := z;
       }
    }

    assert (z >= y);
}
