procedure main() {
    var i: int;
    var k: int;
    assume(0 <= k && k <= 10);
    i := 0;
    while (i < 1000000*k) {
        i := i + k;
    }
    assert(i == 1000000*k);
    
}
