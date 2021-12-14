procedure main() {
    var i: int;
    i := 0;
    while (i < 1000000) {
        i := i + 2;
    }
    assert(i == 1000000);
    
}
