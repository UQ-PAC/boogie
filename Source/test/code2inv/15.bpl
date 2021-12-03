procedure unknown() returns (u: bool);

procedure main()
{
    var x:int; 
    var m:int; 
    var n:int;
    var u:bool;
    x := 0;
    m := 0;

    while (x < n) {
        havoc u;
        if (u) {
            m := x;
        }
        x := x + 1;
    }

       assert ((n > 0) ==> m < n);
       //assert (m >= 0);
}
