$files = Get-ChildItem "test\fib\*" -File -Include "*.bpl"
$timeoutseconds = 60
$root = Get-Location
foreach ($f in $files) {
  $code = {
    param($f, $root)
    cd $root
    $outfile = "test\fib\log\" + ($f.Name -replace "\..+") + ".txt"
    if (!(Test-Path $outfile)) {
      $mathsatlog = "test\fib\log\" + ($f.Name -replace "\..+") + "interpol.txt"
    $z3log = "test\fib\log\" + ($f.Name -replace "\..+") + "z3.txt"
    .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /interpolationDebug:1 /checkInfer /inferinterpolant:smtinterpol /printInstrumented $f.FullName > $outfile 
    }  
  }
  $j = Start-Job -ScriptBlock $code -ArgumentList $f, $root
  if (Wait-Job $j -Timeout $timeoutSeconds) { Receive-Job $j }
  Remove-Job -force $j
  Write-Host $f.Name
}
