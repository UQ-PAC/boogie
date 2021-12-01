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
        call u := unknown();
        if (u) {
            m := x;
        }
        x := x + 1;
    }

    if(n > 0) {
       //assert (m < n);
       assert (m >= 0);
    }
}
