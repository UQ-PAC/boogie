procedure unknown() returns (u: bool);


procedure main()
{
    var z1:int;
    var z2:int;
    var z3:int;
    var x:int; 
    var m:int; 
    var n:int;
    var u:bool;
    x := 1;
    m := 1;

    while (x < n) {
        havoc u;
        if (u) {
            m := x;
        }
        x := x + 1;
    }

       //assert (m < n);
       assert ((n > 1) ==> m >= 1);
}