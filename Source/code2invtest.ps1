$files = Get-ChildItem "test\code2inv"
foreach ($f in $files) {
  $outfile = "test\code2inv\log\" + ($f.Name -replace "\..+") + ".txt"
  $mathsatlog = "test\code2inv\log\" + ($f.Name -replace "\..+") + "interpol.txt"
  $z3log = "test\code2inv\log\" + ($f.Name -replace "\..+") + "z3.txt"
  .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /checkInfer /timeLimit:10 /inferinterpolant:smtinterpol /printInstrumented /interpolationProverLog:$mathsatlog /proverLog:$z3log $f.FullName > $outfile 
  echo $f.Name
}
