$files = Get-ChildItem "test\loops\*" -File -Include "*.bpl"
$timeoutseconds = 60
$root = Get-Location
foreach ($f in $files) {
  $code = {
    param($f, $root)
    cd $root
    $outfile = "test\loops\log\" + ($f.Name -replace "\..+") + ".txt"
    if (!(Test-Path $outfile)) {
      $mathsatlog = "test\loops\log\" + ($f.Name -replace "\..+") + "interpol.txt"
    $z3log = "test\loops\log\" + ($f.Name -replace "\..+") + "z3.txt"
    .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /checkInfer /inferinterpolant:smtinterpol /printInstrumented $f.FullName > $outfile 
    }  
  }
  $j = Start-Job -ScriptBlock $code -ArgumentList $f, $root
  if (Wait-Job $j -Timeout $timeoutSeconds) { Receive-Job $j }
  Remove-Job -force $j
  Write-Host $f.Name
}