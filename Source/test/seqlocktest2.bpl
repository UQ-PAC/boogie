var x: int, z: int, Gamma_x: bool, Gamma_z: bool;

procedure rely();
    modifies x, z, Gamma_x, Gamma_z;
    ensures old(z) == z ==> old(Gamma_z) == Gamma_z;
    ensures old(x) == x ==> old(Gamma_x) == Gamma_x;
    ensures true ==> Gamma_z;
    ensures z mod 2 == 0 ==> Gamma_x;
    ensures old(z) <= z;

procedure read() returns (y: int, Gamma_y: bool)
    modifies x, z, Gamma_x, Gamma_z;
    //requires Gamma_r1 && Gamma_z && r1 <= z && block == 0 && block2 == 0 && z mod 2 == 0 ==> Gamma_x;
{
    var Gamma_r1: bool, Gamma_r2: bool;
    var r1: int;
    var r2: int;
    var block2: int;
    var block: int;
    var old_z: int;
    call rely();
    r1, Gamma_r1 := z, Gamma_z;
    call rely();
    assert Gamma_r1;
    block := 0;
    block2 := 0;

    // assume Gamma_r1 && Gamma_z && r1 <= z && block == 0 && block2 == 0 && z % 2 == 0 ==> Gamma_x;
    while (block2 != -1)
        //invariant r1 <= z;
        //invariant Gamma_r1;
        //invariant block2 == 1 && block == 0 ==> r1 mod 2 == 0 && r1 <= old_z && old_z <= z;
        //invariant block2 == 1 && block == 0 ==> (old_z mod 2 == 0 ==> Gamma_r2);
        //invariant block == -1 ==> z == r1;
       // invariant block == -1 ==> Gamma_r2;
       // invariant block2 == -1 ==> block == -1;
      //  invariant block2 == 0 ==> block != -1;
    {
        if (block2 == 0) {
            if ((r1 mod 2) != 0) {
                call rely();
		        r1, Gamma_r1 := z, Gamma_z;
		        call rely();
            } else {
            	call rely();
		        r2, Gamma_r2 := x, Gamma_x;
                old_z := z;
		        call rely();
    	        assert Gamma_z && Gamma_r1;
    		    block := 0;
    		    block2 := 1;
            }
        } else if (block2 == 1) {
            if (block != -1) {
                if (block == 0) {
                    if (z != r1) {
                        call rely();
			            r1, Gamma_r1 := z, Gamma_z;
        		        call rely();
        		        assert Gamma_r1;
                        block := 1;
                    } else {
                        block := -1;
                    }
                } else if (block == 1) {
                    if ((r1 mod 2) != 0) {
                        call rely();
            		    r1, Gamma_r1 := z, Gamma_z;
            		    call rely();
                    } else {
                        call rely();
        		        r2, Gamma_r2 := x, Gamma_x;
                        old_z := z;
        		        call rely();
                        block := 0;
                    }
                }
            } else {
                block2 := -1;
            }
        }
    }
    call rely();
    assert Gamma_r2;
    y, Gamma_y := r2, Gamma_r2;
}