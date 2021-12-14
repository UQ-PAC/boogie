procedure main() {
    var n: int, sum: int, i: int;
    assume(1 <= n && n <= 1000);
    sum := 0;
    i := 1;
    while(i <= n) {
        sum := sum + i;
        i := i + 1;
    }
    assert(2*sum == n*(n+1));
    
}
