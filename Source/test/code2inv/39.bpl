procedure unknown() returns (u: bool);


procedure main() {
    var n:int;
    var c:int;
    var u:bool;
    c := 0;
    assume (n > 0);

    while (u) {
        if(c == n) {
            c := 1;
        }
        else {
            c := c + 1;
        }
        havoc u;
    }

        //assert( c >= 0);
        assert((c == n) ==> c <= n);
}
