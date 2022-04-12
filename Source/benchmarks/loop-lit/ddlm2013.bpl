procedure main() {
    var i: int,j: int,a: int,b: int;
    var flag: bool;
    a := 0;
    b := 0;
    j := 1;
    if (flag) {
        i := 0;
    } else {
        i := 1;
    }

    while(*) {
        a := a + 1;
        b := b + (j - i);
        i := i + 2;
        if (i mod 2 == 0) {
            j := j + 2;
        } else {
            j := j + 1;
        }
    }
    if (flag) {
        assert(a == b);
    }
    
}
