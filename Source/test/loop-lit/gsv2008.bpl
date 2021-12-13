procedure main() {
    var x: int, y: int;
    x := -50;
    assume(-1000 < y && y < 1000000);
    while (x < 0) {
	x := x + y;
	y := y + 1;
    }
    assert(y > 0);
    
}
