procedure main() {
    var i: int,j: int,k: int;
    i := 1;
    j := 1;
    assume(0 <= k && k <= 1);
    while (i < 1000000) {
	i := i + 1;
	j := j + k;
	k := k - 1;
	assert(1 <= i + k && i + k <= 2 && i >= 1);
    }
    
}
