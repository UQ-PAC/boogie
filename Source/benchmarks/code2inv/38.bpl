


procedure main() {
    var n:int;
    var c:int;
    
    c := 0;
    assume (n > 0);

    while (*) {
        if(c == n) {
            c := 1;
        }
        else {
            c := c + 1;
        }
        
    }

        assert((c == n) ==> c >= 0);
        //assert( c <= n);
}
