$files = Get-ChildItem "test\fib"
foreach ($f in $files) {
  $outfile = "test\fib\log\" + ($f.Name -replace "\..+") + ".txt"
  $mathsatlog = "test\fib\log\" + ($f.Name -replace "\..+") + "mathsat.txt"
  $z3log = "test\fib\log\" + ($f.Name -replace "\..+") + "z3.txt"
  .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /inferinterpolant /printInstrumented /mathSATLog:$mathsatlog /proverLog:$z3log $f.FullName > $outfile 
  echo $f.Name
}
