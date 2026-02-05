# Update-Dtos.ps1
# This script starts the backend server, waits for it to be ready, 
# updates the TypeScript DTOs in the client project, and then shuts down the server.

param(
    [int]$TaskNumber = 1
)

$ErrorActionPreference = "Stop"

# Use TLS 1.2/1.3 and ignore SSL certificates for local dev
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12 -bor [Net.SecurityProtocolType]::Tls13
# Trust all certificates (for local development)
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }
# Use task number for ports (e.g., Task 01 -> 5001, Task 12 -> 5101)
$portSuffix = $TaskNumber.ToString("00")
$baseUrl = "https://localhost:50$portSuffix"
$fallbackUrl = "http://localhost:51$portSuffix"

$metadataPath = "/metadata"
$clientDir = Join-Path $PSScriptRoot "AIInterviewer.Client"
$serverDir = Join-Path $PSScriptRoot "AIInterviewer"


function Test-ServerReady($url) {
    try {
        $response = Invoke-WebRequest -Uri ($url + $metadataPath) -Method Head -TimeoutSec 2 -ErrorAction SilentlyContinue
        return $response.StatusCode -eq 200
    }
    catch {
        return $false
    }
}

# 1. Check if server is already running
Write-Host "Checking if server is already running..." -ForegroundColor Cyan
$alreadyRunning = $false
$targetUrl = ""

if (Test-ServerReady $baseUrl) {
    $targetUrl = $baseUrl
    $alreadyRunning = $true
}
elseif (Test-ServerReady $fallbackUrl) {
    $targetUrl = $fallbackUrl
    $alreadyRunning = $true
}

$serverProcess = $null

if (-not $alreadyRunning) {
    Write-Host "Starting server in $serverDir..." -ForegroundColor Cyan
    # Start dotnet run on specific ports
    $serverProcess = Start-Process dotnet -ArgumentList "run", "--project", "$serverDir", "--urls", "$baseUrl;$fallbackUrl" -NoNewWindow -PassThru
    
    Write-Host "Waiting for server to start successfully..." -ForegroundColor Cyan
    $timeout = 60 # seconds
    $elapsed = 0
    while ($elapsed -lt $timeout) {
        if (Test-ServerReady $baseUrl) {
            Write-Host "`nServer started successfully on $baseUrl!" -ForegroundColor Green
            break
        }
        elseif (Test-ServerReady $fallbackUrl) {
            Write-Host "`nServer started successfully on $fallbackUrl!" -ForegroundColor Green
            break
        }
        Start-Sleep -Seconds 2
        $elapsed += 2
        Write-Host "." -NoNewline
    }

    if ($elapsed -ge $timeout) {
        Write-Error "Timeout waiting for server to start."
        if ($serverProcess) { Stop-ServiceStackServer $serverProcess.Id }
        exit 1
    }
}
else {
    Write-Host "Server is already running on $targetUrl." -ForegroundColor Green
}

function Stop-ServiceStackServer($procId) {
    Write-Host "Stopping server process tree (PID: $procId)..." -ForegroundColor Cyan
    # Use taskkill to kill the entire process tree (-T) forcefully (-F)
    taskkill /F /T /PID $procId 2>$null
}

# 2. Update DTOs
Write-Host "`nUpdating DTOs in $clientDir..." -ForegroundColor Cyan
Push-Location $clientDir
try {
    # Run npm run dtos with the specific URL
    npm run dtos -- --urls $baseUrl
    Write-Host "DTOs updated successfully!" -ForegroundColor Green
}
catch {
    Write-Host "Error: Failed to update DTOs." -ForegroundColor Red
}
finally {
    Pop-Location
}

# 3. Cleanup
if ($serverProcess) {
    Stop-ServiceStackServer $serverProcess.Id
    Write-Host "Server stopped." -ForegroundColor Green
}
else {
    Write-Host "Server was already running; leaving it active." -ForegroundColor Yellow
}

Write-Host "`nDone!" -ForegroundColor Cyan