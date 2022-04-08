var x: int, z: int, Gamma_x: bool, Gamma_z: bool;

procedure rely();
    modifies z, Gamma_x, Gamma_z;
    ensures old(z) == z ==> old(Gamma_z) == Gamma_z;
    ensures old(x) == x ==> old(Gamma_x) == Gamma_x;
    ensures true ==> Gamma_z;
    ensures z mod 2 == 0 ==> Gamma_x;
    ensures old(z) <= z;

procedure read() returns (y: int, Gamma_y: bool)
    modifies x, z, Gamma_x, Gamma_z;
{
    var r1: int, r2: int;
    var Gamma_r1: bool, Gamma_r2: bool;
    //var old_z: int;
    
    call rely();
    r1, Gamma_r1 := z, Gamma_z;
    call rely();
    assert Gamma_r1;
    while (r1 mod 2 != 0)
    invariant r1 + -2 * int(1 / 2 * real(r1)) != 0 || (Gamma_r1 && Gamma_x);
    // _@block == 0
        //invariant r1 <= z;
        //invariant Gamma_r1;
    {
        call rely();
        r1, Gamma_r1 := z, Gamma_z;
        call rely();
    }
    call rely();
    r2, Gamma_r2 := x, Gamma_x;
    //old_z := z; // useful heuristic - create temporary variable to capture state of control variable at assignment to its controlled variable 
    call rely();
    assert Gamma_z && Gamma_r1;
    while (z != r1)
    invariant Gamma_r2;
    //invariant _@block == 2
        //invariant r1 mod 2 == 0;
        //invariant r1 <= old_z && old_z <= z;
        //invariant old_z mod 2 == 0 ==> Gamma_r2;
    {
        call rely();
        r1, Gamma_r1 := z, Gamma_z;
        call rely();
        assert Gamma_r1;
        while (r1 mod 2 != 0)
        invariant r1 + -2 * int(1 / 2 * real(r1)) != 0 || (Gamma_r1 && Gamma_x);
        // invariant _@block == 1
            //invariant r1 <= z;
        {
            call rely();
            r1, Gamma_r1 := z, Gamma_z;
            call rely();
        }
        call rely();
        r2, Gamma_r2 := x, Gamma_x;
        //old_z := z;
        call rely();
    }
    call rely();
    assert Gamma_r2;
    y, Gamma_y := r2, Gamma_r2;
}
