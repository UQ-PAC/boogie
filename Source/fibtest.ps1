$files = Get-ChildItem "test\fib\*" -File -Include "*.bpl"
$timeoutseconds = 60
$root = Get-Location
foreach ($f in $files) {
  $code = {
    param($f, $root)
    cd $root
    $outfile = "test\fib\mathsatqe\" + ($f.Name -replace "\..+") + ".txt"
    if (!(Test-Path $outfile)) {
      .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /interpolationQE:qe /interpolationDebug:1 /checkInfer /inferInterpolant:mathsat /printInstrumented $f.FullName > $outfile 
    }
  }
  $j = Start-Job -ScriptBlock $code -ArgumentList $f, $root
  if (Wait-Job $j -Timeout $timeoutSeconds) { Receive-Job $j }
  Remove-Job -force $j
  $code = {
    param($f, $root)
    cd $root
    $outfile = "test\fib\mathsatqe2\" + ($f.Name -replace "\..+") + ".txt"
    if (!(Test-Path $outfile)) {
      .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /interpolationQE:qe2 /interpolationDebug:1 /checkInfer /inferInterpolant:mathsat /printInstrumented $f.FullName > $outfile 
    }
  }
  $j = Start-Job -ScriptBlock $code -ArgumentList $f, $root
  if (Wait-Job $j -Timeout $timeoutSeconds) { Receive-Job $j }
  Remove-Job -force $j
  $code = {
    param($f, $root)
    cd $root
    $outfile = "test\fib\mathsatqerec\" + ($f.Name -replace "\..+") + ".txt"
    if (!(Test-Path $outfile)) {
      .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /interpolationQE:qe_rec /interpolationDebug:1 /checkInfer /inferInterpolant:mathsat /printInstrumented $f.FullName > $outfile 
    }
  }
  $j = Start-Job -ScriptBlock $code -ArgumentList $f, $root
  if (Wait-Job $j -Timeout $timeoutSeconds) { Receive-Job $j }
  Remove-Job -force $j
  $code = {
    param($f, $root)
    cd $root
    $outfile = "test\fib\smtinterpolqe\" + ($f.Name -replace "\..+") + ".txt"
    if (!(Test-Path $outfile)) {
      .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /interpolationQE:qe /interpolationDebug:1 /checkInfer /inferInterpolant:smtinterpol /printInstrumented $f.FullName > $outfile 
    }
  }
  $j = Start-Job -ScriptBlock $code -ArgumentList $f, $root
  if (Wait-Job $j -Timeout $timeoutSeconds) { Receive-Job $j }
  Remove-Job -force $j
  $code = {
    param($f, $root)
    cd $root
    $outfile = "test\fib\smtinterpolqe2\" + ($f.Name -replace "\..+") + ".txt"
    if (!(Test-Path $outfile)) {
      .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /interpolationQE:qe2 /interpolationDebug:1 /checkInfer /inferInterpolant:smtinterpol /printInstrumented $f.FullName > $outfile 
    }
  }
  $j = Start-Job -ScriptBlock $code -ArgumentList $f, $root
  if (Wait-Job $j -Timeout $timeoutSeconds) { Receive-Job $j }
  Remove-Job -force $j
  $code = {
    param($f, $root)
    cd $root
    $outfile = "test\fib\smtinterpolqerec\" + ($f.Name -replace "\..+") + ".txt"
    if (!(Test-Path $outfile)) {
      .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /interpolationQE:qe_rec /interpolationDebug:1 /checkInfer /inferInterpolant:smtinterpol /printInstrumented $f.FullName > $outfile 
    }  
  }
  $j = Start-Job -ScriptBlock $code -ArgumentList $f, $root
  if (Wait-Job $j -Timeout $timeoutSeconds) { Receive-Job $j }
  Remove-Job -force $j


  Write-Host $f.Name
}
