procedure unknown() returns (u: bool);

procedure main() {
    var i:int; 
    var j:int;
    var c:int;
    var t:int;
    var u:bool;
    i := 0;
    while(u) {
        if(c > 48) {
            if (c < 57) {
                j := i + i;
                t := c - 48;
                i := j + t;
            }
        }
        havoc u;
    } 
    assert (i >= 0);
}
