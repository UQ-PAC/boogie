$files = Get-ChildItem "test\fib"
foreach ($f in $files) {
  $outfile = "test\fib\log\" + ($f.Name -replace "\..+") + ".txt"
  $mathsatlog = "test\fib\log\" + ($f.Name -replace "\..+") + "interpol.txt"
  $z3log = "test\fib\log\" + ($f.Name -replace "\..+") + "z3.txt"
  .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /checkInfer /inferinterpolant:smtinterpol /printInstrumented /interpolationProverLog:$mathsatlog /proverLog:$z3log $f.FullName > $outfile 
  echo $f.Name
}
