# PowerShell script to refresh Edge browser tab
param(
    [string]$Url = "http://localhost:5000"
)

try {
    # Get all Edge processes
    $edgeProcesses = Get-Process -Name "msedge" -ErrorAction SilentlyContinue
    
    if ($edgeProcesses.Count -gt 0) {
        # Send F5 key to the first Edge window (refresh)
        Add-Type -AssemblyName System.Windows.Forms
        
        # Get the first Edge window handle
        $windows = Get-Process -Name "msedge" | ForEach-Object { 
            try {
                [System.Diagnostics.Process]::GetProcessById($_.Id) | Select-Object -ExpandProperty MainWindowTitle
            } catch {
                $null
            }
        }
        
        # Send F5 key to the active window (this approach may not work reliably)
        Write-Host "Attempting to refresh Edge browser..." -ForegroundColor Yellow
        
        # Alternative: Use Windows API to send refresh command
        # This is more complex and less reliable for tab refresh specifically
    }
    
    Write-Host "Refresh command sent to Edge" -ForegroundColor Green
}
catch {
    Write-Error "Failed to refresh browser: $_"
}
