
procedure main() {
    var x:int; 
    var y:int;
    x := 1;
    
    while (x <= 100) {
        y := 100 - x;
        x := x +1;
    }

    assert (y >= 0);
    //assert (y < 100);
}