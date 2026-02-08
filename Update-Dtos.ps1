# Update-Dtos.ps1
# This script starts the backend server in a new process, waits briefly,
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

$clientDir = Join-Path $PSScriptRoot "AIInterviewer.Client"
$serverDir = Join-Path $PSScriptRoot "AIInterviewer"
$appDataDir = Join-Path $serverDir "App_Data"

function Stop-ServiceStackServer($procId) {
    Write-Host "Stopping server process tree (PID: $procId)..." -ForegroundColor Cyan
    # Use taskkill to kill the entire process tree (-T) forcefully (-F)
    taskkill /F /T /PID $procId 2>$null
}

# 1. Ensure App_Data exists (required for app startup/migrations)
if (-not (Test-Path -Path $appDataDir -PathType Container)) {
    Write-Host "Missing App_Data folder in $serverDir." -ForegroundColor Red
    Write-Host "Create App_Data and run 'npm run migrate' from the AIInterviewer folder, then try again." -ForegroundColor Red
    exit 1
}

# 2. Start server in a new process and wait briefly
Write-Host "Starting server in $serverDir..." -ForegroundColor Cyan
$serverProcess = Start-Process dotnet -ArgumentList "run", "--project", "$serverDir", "--urls", "$baseUrl;$fallbackUrl" -WorkingDirectory $serverDir -NoNewWindow -PassThru

Write-Host "Waiting 15 seconds for server to start..." -ForegroundColor Cyan
Start-Sleep -Seconds 15

# 3. Update DTOs
Write-Host "`nUpdating DTOs in $clientDir..." -ForegroundColor Cyan
Push-Location $clientDir
try {
    # Run pnpm run update-dtos with the specific URL and output path
    pnpm run update-dtos $baseUrl src/lib/dtos.ts
    Write-Host "DTOs updated successfully!" -ForegroundColor Green
}
catch {
    Write-Host "Error: Failed to update DTOs." -ForegroundColor Red
}
finally {
    Pop-Location
}

# 4. Cleanup
if ($serverProcess) {
    Stop-ServiceStackServer $serverProcess.Id
    Write-Host "Server stopped." -ForegroundColor Green
}

Write-Host "`nDone!" -ForegroundColor Cyan
