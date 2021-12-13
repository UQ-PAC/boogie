procedure main() {
    var i: int, n: int, a: int, b: int;
    var u: bool;
    i := 0;
    a := 0;
    b := 0;
    assume(n >= 0 && n <= 1000000);
    while (i < n) {
        havoc u;
  
        if(u) {
            a := a + 1;
            b := b + 2;
        } else {
            a := a + 2;
            b := b + 1;
        }
        i := i + 1;
    }
    assert(a + b == 3*n);
    
}
