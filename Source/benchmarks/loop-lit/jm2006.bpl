procedure main() {
    var i: int, j: int, x: int, y: int;
    assume(i >= 0 && j >= 0);
    x := i;
    y := j;
    while(x != 0) {
        x := x - 1;
        y := y - 1;
    }

    if (i == j) {
        assert(y == 0);
    }
    
}
