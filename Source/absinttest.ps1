$files = Get-ChildItem "test\fib"
foreach ($f in $files) {
  $outfile = "test\fib\absintlog\" + ($f.Name -replace "\..+") + ".txt"
  .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /infer:j /printInstrumented $f.FullName > $outfile 
  echo $f.Name
}
