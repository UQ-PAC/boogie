procedure main() {
    var x: int,y: int;
    x := 0;
    y := 0;
    while (true) {
        if (x < 50) {
            y := y + 1;
        } else {
            y := y - 1;
        }
        if (y < 0) {
          break;
        }
        x := x + 1;
    }
    assert(x == 100);
    
}
