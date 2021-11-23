$files = Get-ChildItem "test\fib"
foreach ($f in $files) {
  $outfile = "test\fib\log\" + ($f.Name -replace "\..+") + ".txt"
  .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /inferinterpolant /printInstrumented $f.FullName > $outfile 
}
