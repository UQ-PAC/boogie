procedure main() {
    var x:int, y: int;
    x := 0;
    y := 50;
    while(x < 100) {
        if (x < 50) {
            x := x + 1;
        } else {
            x := x + 1;
            y := y + 1;
        }
    }
    assert(y == 100);
    
}
