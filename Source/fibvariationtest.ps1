$files = Get-ChildItem "test\fibvariation\*" -File -Include "*.bpl"
$timeoutseconds = 60
$root = Get-Location
if (!(Test-Path -path "test\fibvariation\mathsatqe\")) {
  New-Item -ItemType Directory -Force -Path "test\fibvariation\mathsatqe\"
}
if (!(Test-Path -path "test\fibvariation\mathsatqe2\")) {
  New-Item -ItemType Directory -Force -Path "test\fibvariation\mathsatqe2\"
}
if (!(Test-Path -path "test\fibvariation\mathsatqerec\")) {
  New-Item -ItemType Directory -Force -Path "test\fibvariation\mathsatqerec\"
}
if (!(Test-Path -path "test\fibvariation\smtinterpolqe\")) {
  New-Item -ItemType Directory -Force -Path "test\fibvariation\smtinterpolqe\"
}
if (!(Test-Path -path "test\fibvariation\smtinterpolqe2\")) {
  New-Item -ItemType Directory -Force -Path "test\fibvariation\smtinterpolqe2\"
}
if (!(Test-Path -path "test\fibvariation\smtinterpolqerec\")) {
  New-Item -ItemType Directory -Force -Path "test\fibvariation\smtinterpolqerec\"
}
 
foreach ($f in $files) {
  $code = {
    param($f, $root)
    cd $root
    $outfile = "test\fibvariation\mathsatqe\" + ($f.Name -replace "\..+") + ".txt"
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
    $outfile = "test\fibvariation\mathsatqe2\" + ($f.Name -replace "\..+") + ".txt"
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
    $outfile = "test\fibvariation\mathsatqerec\" + ($f.Name -replace "\..+") + ".txt"
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
    $outfile = "test\fibvariation\smtinterpolqe\" + ($f.Name -replace "\..+") + ".txt"
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
    $outfile = "test\fibvariation\smtinterpolqe2\" + ($f.Name -replace "\..+") + ".txt"
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
    $outfile = "test\fibvariation\smtinterpolqerec\" + ($f.Name -replace "\..+") + ".txt"
    if (!(Test-Path $outfile)) {
      .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /interpolationQE:qe_rec /interpolationDebug:1 /checkInfer /inferInterpolant:smtinterpol /printInstrumented $f.FullName > $outfile 
    }  
  }
  $j = Start-Job -ScriptBlock $code -ArgumentList $f, $root
  if (Wait-Job $j -Timeout $timeoutSeconds) { Receive-Job $j }
  Remove-Job -force $j


  Write-Host $f.Name
}
