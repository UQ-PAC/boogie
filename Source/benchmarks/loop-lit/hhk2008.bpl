procedure main() {
    var a: int;
    var b: int;
    var res: int, cnt: int;
    assume(a <= 1000000);
    assume(0 <= b && b <= 1000000);
    res := a;
    cnt := b;
    while (cnt > 0) {
        cnt := cnt - 1;
        res := res + 1;
    }
    assert(res == a + b);
    
}
