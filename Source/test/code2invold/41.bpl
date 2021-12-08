procedure unknown() returns (u: bool);

procedure main() {
  // variable declarations
  var c:int;
  var n:int;
  var u:bool;
  // pre-conditions
  c := 0;
  assume((n > 0));
  havoc u;
  // loop body

  n - c >= 0
  
  while (u)
  {
      havoc u;
      if (u) {
        if ( (c > n) )
        {
        c  :=  (c + 1);
        }
      } else {
        if ( (c == n) )
        {
        c  :=  1;
        }
      }
      havoc u;
  }
  // post-condition
  assert((c != n) ==> (c <= n) );
}

(exists c' :: c == 1 && c' == n && u'' && exists u' :: n - c' > - 1 && u')
c == 1 && n - n > - 1
c == 1 
c == 1


(c != n && u'' && exists u' :: n - c > - 1 && u'')
c != n && n - c > - 1

(exists c' :: c == c' + 1 && c' > n && u'' && exists u' :: n - c' > - 1 && u' )
c - 1 > n && n - c > 0

 (c <= n && u'' && exists u' :: n - c > - 1 && u')
c <= n && n - c > -1

c == 1 ||
(c != n && n - c > - 1) ||
(c <= n && n - c > -1) 
==> 

n - c > -1