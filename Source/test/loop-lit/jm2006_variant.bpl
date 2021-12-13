procedure main() {
    var i: int, j: int, x: int, y: int, z: int;
    assume(i >= 0 && i <= 1000000);
    assume(j >= 0);
    x := i;
    y := j;

    z := 0;
    while(x != 0) {
        x := x - 1;
        y := y - 2;
        z := z + 1;
    }
    if (i == j) {
        assert(y == -z);
    }
    
}
