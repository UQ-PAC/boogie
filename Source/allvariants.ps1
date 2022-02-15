function RunBoogie {

  param (
    $f, $folder, $timeoutSeconds, $qe, $solver, $direction
  )

  if ($direction -eq "forward") {
    $squeeze = "/forwardSqueeze"
  } else {
    $squeeze = ""
  }

  $boogieArgs = $squeeze + " /interpolationQE:" + $qe + " /interpolationDebug:3 /inferInterpolant:" + $solver + " " + $f.FullName
  $outfile = $folder + $direction + $solver + $qe + "\" + ($f.Name -replace "\..+") + ".txt"
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

if (!(Test-Path -path ($folder + "backwardmathsatqe\"))) {
  New-Item -ItemType Directory -Force -Path ($folder + "backwardmathsatqe\")
}
if (!(Test-Path -path ($folder + "backwardmathsatqe2\"))) {
  New-Item -ItemType Directory -Force -Path ($folder + "backwardmathsatqe2\")
}
if (!(Test-Path -path ($folder + "backwardmathsatqe_rec"))) {
  New-Item -ItemType Directory -Force -Path ($folder + "backwardmathsatqe_rec")
}
if (!(Test-Path -path ($folder + "backwardsmtinterpolqe\"))) {
  New-Item -ItemType Directory -Force -Path ($folder + "backwardsmtinterpolqe\")
}
if (!(Test-Path -path ($folder + "backwardsmtinterpolqe2\"))) {
  New-Item -ItemType Directory -Force -Path ($folder + "backwardsmtinterpolqe2\")
}
if (!(Test-Path -path ($folder + "backwardsmtinterpolqe_rec"))) {
  New-Item -ItemType Directory -Force -Path ($folder + "backwardsmtinterpolqe_rec")
}
  <#
if (!(Test-Path -path ($folder + "forwardmathsatqe\"))) {
  New-Item -ItemType Directory -Force -Path ($folder + "forwardmathsatqe\")
}
if (!(Test-Path -path ($folder + "forwardmathsatqe2\"))) {
  New-Item -ItemType Directory -Force -Path ($folder + "forwardmathsatqe2\")
}
if (!(Test-Path -path ($folder + "forwardmathsatqe_rec"))) {
  New-Item -ItemType Directory -Force -Path ($folder + "forwardmathsatqe_rec")
}
if (!(Test-Path -path ($folder + "forwardsmtinterpolqe\"))) {
  New-Item -ItemType Directory -Force -Path ($folder + "forwardsmtinterpolqe\")
}
if (!(Test-Path -path ($folder + "forwardsmtinterpolqe2\"))) {
  New-Item -ItemType Directory -Force -Path ($folder + "forwardsmtinterpolqe2\")
}
if (!(Test-Path -path ($folder + "forwardsmtinterpolqe_rec"))) {
  New-Item -ItemType Directory -Force -Path ($folder + "forwardsmtinterpolqe_rec")
}
  #>


foreach ($f in $files) {
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe" -solver "mathsat" -direction "backward"
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe2" -solver "mathsat" -direction "backward"
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe_rec" -solver "mathsat" -direction "backward"
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe" -solver "smtinterpol" -direction "backward"
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe2" -solver "smtinterpol" -direction "backward"
  RunBoogie -f $f -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe_rec" -solver "smtinterpol" -direction "backward"
  <#
  RunBoogie -f $f -root $root -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe" -solver "mathsat" -direction "forward"
  RunBoogie -f $f -root $root -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe2" -solver "mathsat" -direction "forward"
  RunBoogie -f $f -root $root -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe_rec" -solver "mathsat" -direction "forward"
  RunBoogie -f $f -root $root -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe" -solver "smtinterpol" -direction "forward"
  RunBoogie -f $f -root $root -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe2" -solver "smtinterpol" -direction "forward"
  RunBoogie -f $f -root $root -folder $folder -timeoutSeconds $timeoutSeconds -qe "qe_rec" -solver "smtinterpol" -direction "forward"
  #>
  Write-Host $f.Name
}