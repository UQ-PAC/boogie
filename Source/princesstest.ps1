function RunBoogie {

  param (
    $f, $folder, $timeoutSeconds, $qe, $solver
  )

  $boogieArgs = " /interpolationQE:" + $qe + " /interpolationDebug:3 /inferInterpolant:" + $solver + " " + $f.FullName
  $outfile = $folder + $solver + $qe + "\" + ($f.Name -replace "\..+") + ".txt"
  if (!(Test-Path $outfile)) {
    $proc = Start-Process -FilePath "BoogieDriver/bin/Debug/net5.0/BoogieDriver.exe" -PassThru -NoNewWindow -ArgumentList $boogieArgs -RedirectStandardOutput $outfile
    $proc | Wait-Process -Timeout $timeoutSeconds -ErrorAction SilentlyContinue -ErrorVariable timeouted

    $appended = $false

    if ($timeouted) {
      $children = Get-Process | Where-Object { ($proc.Id -eq $_.Parent.Id) }
      Stop-Process -InputObject $proc
      $children | ForEach-Object { $_.Kill() }
      Start-Sleep -s 1 # necessary for processes to die
      while (!$appended) {
        try {
          Add-Content -Path $outfile "timeout" -ErrorAction Stop
          $appended = $true
        } catch {
          Write-Host "waiting to write timeout"
          Start-Sleep -s 0.5
        }
      }
    } elseif ($proc.ExitCode -ne 0) {
      Start-Sleep -s 1 # necessary for processes to die
      while (!$appended) {
        try {
          Add-Content -Path $outfile "error" -ErrorAction Stop
          $appended = $true
        } catch {
          Write-Host "waiting to write error"
          Start-Sleep -s 0.5
        }
      }
    }
  }
  Write-Host $outfile
}


$folder = $args[0]
$files = Get-ChildItem ($folder + "*") -File -Include "*.bpl"
$timeoutseconds = 60
if (!(Test-Path -path ($folder + "princessqe\"))) {
  New-Item -ItemType Directory -Force -Path ($folder + "princessqe\")
}

foreach ($f in $files) {
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe" -solver "princess"
}