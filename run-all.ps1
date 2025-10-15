Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Stop-Port {
  param([int]$Port)
  try {
    $conns = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue
    if ($conns) {
      $pid = ($conns | Select-Object -First 1).OwningProcess
      Write-Host ("Stopping PID {0} on port {1}" -f $pid, $Port)
      Stop-Process -Id $pid -Force -ErrorAction Stop
      Write-Host ("Stopped PID {0}" -f $pid)
    } else {
      Write-Host ("No listener on port {0}" -f $Port)
    }
  } catch {
    Write-Host ("Could not stop listener on port {0}: {1}" -f $Port, $_)
  }
}

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$apiDir = Join-Path $root 'Backend\PetCloud.Api'
$frontDir = Join-Path $root 'Frontend'

# Stable ports
$httpsPort = 54433
$httpPort = 54434
$frontPort = 5500

Write-Host '--- Cleaning ports ---'
Stop-Port -Port $httpsPort
Stop-Port -Port $httpPort
Stop-Port -Port $frontPort

Write-Host '--- Starting API ---'
# Use launchSettings (already configured for these ports)
$apiCmd = "cd `"$apiDir`"; dotnet run"
Start-Process -FilePath 'powershell.exe' -ArgumentList '-NoExit','-Command', $apiCmd -WindowStyle Minimized | Out-Null

# Wait for the port to be listening
Write-Host 'Waiting for API to listen...'
$maxWait = 60
$waited = 0
while ($waited -lt $maxWait) {
  $t = Test-NetConnection -ComputerName 'localhost' -Port $httpsPort -WarningAction SilentlyContinue
  if ($t.TcpTestSucceeded) { break }
  Start-Sleep -Seconds 1
  $waited++
}
if ($waited -ge $maxWait) { Write-Warning 'API did not open the port in time. Continuing anyway...'}

Write-Host '--- Starting Frontend server ---'
Push-Location $frontDir
$frontUrl = "http://localhost:$frontPort"
$apiHttpsUrl = "https://localhost:$httpsPort"

function Start-Frontend {
  if (Get-Command dotnet-serve -ErrorAction SilentlyContinue) {
    Start-Process -FilePath 'dotnet-serve' -ArgumentList '-p', $frontPort.ToString() -WindowStyle Minimized | Out-Null
    return $true
  }
  # Try user global tools path
  $dotnetServePath = Join-Path $env:USERPROFILE '.dotnet\tools\dotnet-serve.exe'
  if (Test-Path $dotnetServePath) {
    Start-Process -FilePath $dotnetServePath -ArgumentList '-p', $frontPort.ToString() -WindowStyle Minimized | Out-Null
    return $true
  }
  # Try npx serve
  if (Get-Command npx -ErrorAction SilentlyContinue) {
    Start-Process -FilePath 'npx' -ArgumentList 'serve','-l', $frontPort.ToString(), '.' -WindowStyle Minimized | Out-Null
    return $true
  }
  return $false
}

$served = Start-Frontend
Pop-Location

Start-Sleep -Seconds 2

Write-Host '--- Opening browser ---'
if ($served) {
  # Set apiBase then go to login
  $setUrl = "$frontUrl/set-api.html?api=$([Uri]::EscapeDataString($apiHttpsUrl))&next=login.html"
  Start-Process $setUrl | Out-Null
} else {
  # Fallback: open via file:// scheme
  $setFile = Join-Path $frontDir 'set-api.html'
  $fileUrl = 'file:///' + ($setFile -replace '\\','/') + "?api=$([Uri]::EscapeDataString($apiHttpsUrl))&next=login.html"
  Start-Process $fileUrl | Out-Null
  Write-Warning 'Frontend not served via HTTP. Using file:// fallback. For best results, install dotnet-serve: dotnet tool install --global dotnet-serve'
}

Write-Host 'Ready. Credentials de prueba:'
Write-Host '  Dueño:      dueno@demo.com   /  Dueno1234!'
Write-Host '  Veterinario: vet@demo.com     /  Vet1234!'
