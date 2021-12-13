procedure main() {
    var x: int, y: int, u: bool;
    x := 1;
    y := 0;
    while (y < 1000 && u) {
        x := x + y;
        y := y + 1;
        havoc u;
    }
    assert(x >= y);
    
}
