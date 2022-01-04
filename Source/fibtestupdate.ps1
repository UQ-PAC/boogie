$files = Get-ChildItem "test\fib\*" -File -Include "*.bpl"
$timeoutseconds = 60
$root = Get-Location
if (!(Test-Path -path "test\fib\extracheckmathsatqe\")) {
  New-Item -ItemType Directory -Force -Path "test\fib\extracheckmathsatqe\"
}
if (!(Test-Path -path "test\fib\extracheckmathsatqe2\")) {
  New-Item -ItemType Directory -Force -Path "test\fib\extracheckmathsatqe2\"
}
if (!(Test-Path -path "test\fib\extracheckmathsatqerec\")) {
  New-Item -ItemType Directory -Force -Path "test\fib\extracheckmathsatqerec\"
}
if (!(Test-Path -path "test\fib\extrachecksmtinterpolqe\")) {
  New-Item -ItemType Directory -Force -Path "test\fib\extrachecksmtinterpolqe\"
}
if (!(Test-Path -path "test\fib\extrachecksmtinterpolqe2\")) {
  New-Item -ItemType Directory -Force -Path "test\fib\extrachecksmtinterpolqe2\"
}
if (!(Test-Path -path "test\fib\extrachecksmtinterpolqerec\")) {
  New-Item -ItemType Directory -Force -Path "test\fib\extrachecksmtinterpolqerec\"
}$files = Get-ChildItem "test\fib\*" -File -Include "*.bpl"
$timeoutseconds = 60
$root = Get-Location
foreach ($f in $files) {
  $code = {
    param($f, $root)
    cd $root
    $outfile = "test\fib\extracheckmathsatqe\" + ($f.Name -replace "\..+") + ".txt"
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
    $outfile = "test\fib\extracheckmathsatqe2\" + ($f.Name -replace "\..+") + ".txt"
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
    $outfile = "test\fib\extracheckmathsatqerec\" + ($f.Name -replace "\..+") + ".txt"
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
    $outfile = "test\fib\extrachecksmtinterpolqe\" + ($f.Name -replace "\..+") + ".txt"
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
    $outfile = "test\fib\extrachecksmtinterpolqe2\" + ($f.Name -replace "\..+") + ".txt"
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
    $outfile = "test\fib\extrachecksmtinterpolqerec\" + ($f.Name -replace "\..+") + ".txt"
    if (!(Test-Path $outfile)) {
      .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /interpolationQE:qe_rec /interpolationDebug:1 /checkInfer /inferInterpolant:smtinterpol /printInstrumented $f.FullName > $outfile 
    }  
  }
  $j = Start-Job -ScriptBlock $code -ArgumentList $f, $root
  if (Wait-Job $j -Timeout $timeoutSeconds) { Receive-Job $j }
  Remove-Job -force $j


  Write-Host $f.Name
}
