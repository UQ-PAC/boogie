var x: int, z: int, Gamma_x: bool, Gamma_z: bool;

procedure rely();
    modifies x, z, Gamma_x, Gamma_z;
    ensures old(z) == z ==> old(Gamma_z) == Gamma_z;
    ensures old(x) == x ==> old(Gamma_x) == Gamma_x;
    ensures true ==> Gamma_z;
    ensures z mod 2 == 0 ==> Gamma_x;
    ensures old(z) <= z;

procedure read() returns ()
    modifies x, z, Gamma_x, Gamma_z;
{
    var r1: int, r2: int;
    var Gamma_r1: bool, Gamma_r2: bool;

    call rely();
    r1, Gamma_r1 := z, Gamma_z;
    call rely();
    assert Gamma_r1;
    while (r1 mod 2 != 0)
        invariant r1 <= z;
        invariant Gamma_r1;
    {
        call rely();
        r1, Gamma_r1 := z, Gamma_z;
        call rely();
    }
    call rely();
    r2, Gamma_r2 := x, Gamma_x;
    call rely();
    assert Gamma_z && Gamma_r1;
}