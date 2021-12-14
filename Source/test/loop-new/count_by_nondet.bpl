procedure main() {
    var i: int;
    var k: int;
    var j: int;
    i := 0;
    k := 0;
    while(i < 1000000) {
        havoc j;
        assume(1 <= j && j < 1000000);
        i := i + j;
        k := k + 1;
    }
    assert(k <= 1000000);
    
}
