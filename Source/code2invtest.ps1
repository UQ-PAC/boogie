$files = Get-ChildItem "test\code2inv"
$timeoutseconds = 60
foreach ($f in $files) {
  $code = {
    param($f)
    cd D:\Work\boogie\Source
    $outfile = "test\code2inv\log\" + ($f.Name -replace "\..+") + ".txt"
    if (!(Test-Path $outfile)) {
      $mathsatlog = "test\code2inv\log\" + ($f.Name -replace "\..+") + "interpol.txt"
    $z3log = "test\code2inv\log\" + ($f.Name -replace "\..+") + "z3.txt"
    Write-Host $f.Name
    .\BoogieDriver\bin\Debug\net5.0\BoogieDriver.exe /checkInfer /timeLimit:10 /inferinterpolant:smtinterpol /printInstrumented $f.FullName > $outfile 
    }  
  }
  $j = Start-Job -ScriptBlock $code -ArgumentList $f
  if (Wait-Job $j -Timeout $timeoutSeconds) { Receive-Job $j }
  Remove-Job -force $j
}
