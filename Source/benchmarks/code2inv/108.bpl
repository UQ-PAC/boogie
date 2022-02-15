
procedure main() {
    var a:int;
    var c:int;
    var m:int;
    var j:int;
    var k:int;

    assume(a <= m);
    j := 0;
    k := 0;

    while ( k < c) {
        if(m < a) {
            m := a;
        }
        k := k + 1;
    }

    assert( a <=  m);
}
