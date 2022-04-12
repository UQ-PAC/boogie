procedure main() {
    var k: int;
    var i: int, n: int;
    i := 0;
    n := 0;
    assume(k <= 1000000 && k >= -1000000);
    while(i < 2*k) {
        if (i mod 2 == 0) {
            n := n + 1;
        }
        i := i + 1;
    }
    assert(k < 0 || n == k);
    
}
