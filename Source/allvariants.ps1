function RunBoogie {

  param (
    $f, $folder, $timeoutSeconds, $qe, $solver, [switch] $forward, [switch] $passive
  )
  
  if ($forward) {
    $forwardflag = " /interpolationDirection:forward"
    $forwardout = "forward"
  } else {
    $forwardflag = ""
    $forwardout = ""
  }

  if ($passive) {
    $passiveflag = " /passifyInterpolation"
    $passiveout = "passive"
  } else {
    $passiveflag = ""
    $passiveout = ""
  }

  $benchmarkset = Split-Path -Path $folder -Leaf

  $outfolder = $folder + "..\results\" + $benchmarkset + "\" + $solver + $qe + $forwardout + $passiveout + "\"
  if (!(Test-Path -path $outfolder)) {
    New-Item -ItemType Directory -Force -Path $outfolder
  }

  $boogieArgs = $forwardflag + $passiveflag + " /interpolationQE:" + $qe + " /interpolationDebug:3 /inferInterpolant:" + $solver + " " + $f.FullName
  $outfile = $outfolder + ($f.Name -replace "\..+") + ".txt"
  if (!(Test-Path $outfile)) {
    $proc = Start-Process -FilePath "BoogieDriver/bin/Debug/net5.0/BoogieDriver.exe" -NoNewWindow -PassThru -ArgumentList $boogieArgs -RedirectStandardOutput $outfile
    $proc | Wait-Process -Timeout $timeoutSeconds -ErrorAction SilentlyContinue -ErrorVariable timeouted

    $appended = $false
    if ($timeouted) {
      $children = Get-Process | Where-Object { ($proc.Id -eq $_.Parent.Id) }
      Stop-Process -InputObject $proc
      $children | ForEach-Object { $_.Kill() }
      $children | ForEach-Object { $_.WaitForExit() }
      $proc.WaitForExit()
      while (!$appended) {
        try {
          Add-Content -Path $outfile "timeout" -ErrorAction Stop
          $appended = $true
        } catch {
          Write-Host "waiting to write timeout"
          Start-Sleep -s 1
        }
      }
    } elseif ($proc.ExitCode -ne 0) {
      while (!$appended) {
        try {
          Add-Content -Path $outfile "error" -ErrorAction Stop
          $appended = $true
        } catch {
          Write-Host "waiting to write error"
          Start-Sleep -s 1
        }
      }
    }
  }
  Write-Host $outfile
}


$folder = $args[0]
$files = Get-ChildItem ($folder + "*") -File -Include "*.bpl"
$timeoutseconds = 60

foreach ($f in $files) {
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe" -solver "mathsat" -forward
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe" -solver "smtinterpol" -forward
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe" -solver "princess" -forward
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe2" -solver "mathsat" -forward
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe2" -solver "smtinterpol" -forward
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe2" -solver "princess" -forward
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe_rec" -solver "mathsat" -forward
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe_rec" -solver "smtinterpol" -forward
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe_rec" -solver "princess" -forward
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe" -solver "mathsat"
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe" -solver "smtinterpol"
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe" -solver "princess"
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe2" -solver "mathsat" 
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe2" -solver "smtinterpol" 
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe2" -solver "princess"
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe_rec" -solver "mathsat" 
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe_rec" -solver "smtinterpol" 
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe_rec" -solver "princess"
}