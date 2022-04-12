procedure main() {
    var lo: int, mid: int, hi: int;
    lo := 0;
    assume(mid > 0 && mid <= 1000000);
    hi := 2*mid;
    while (mid > 0) {
        lo := lo + 1;
        hi := hi - 1;
        mid := mid - 1;
    }
    assert(lo == hi);
    
}
