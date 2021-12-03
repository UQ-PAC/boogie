procedure unknown() returns (u: bool);

procedure main()
{
    var x:int; 
    var m:int;
    var n:int;
    var u:bool;
    m := 0;
    x := 0;

    while (x < n) {
        havoc u;
        if (u) {
            m := x;
        }
        x := x + 1;
    }

       //assert (m < n);
       assert ((n > 0) ==> m >= 0);
}
