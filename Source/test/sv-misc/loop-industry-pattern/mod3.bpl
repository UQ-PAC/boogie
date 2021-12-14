procedure main(){
  var x: int;
  var y: int;
  var u: bool;
  assume (x >= 0);
  y := 1;
  
  while(u){
    if(x mod 3 == 1){
      x := x + 2;
      y := 0;
    } else{
      if(x mod 3 == 2){
        x := x + 1;
        y := 0;
      } else{
        havoc u;
	      if(u){
	        x := x + 4;
          y := 1;
        } else{
	        x := x + 5;
          y := 1;
        }
      }
    }
  }
  if(y == 0) {
    assert(x mod 3 == 0);
  }
  
}

  
