# A watcher that, upon files changing, copies them from the addon subdirectories to the gamemode's ones

cmd.exe /c asset-copier.bat

$watcher = New-Object System.IO.FileSystemWatcher
$watcher.Path = (Get-Location)
$watcher.Filter = '*'
$watcher.IncludeSubdirectories = $true

$action = {
    $path = $event.SourceEventArgs.FullPath
    $changeType = $event.SourceEventArgs.ChangeType
    $logline = "$(Get-Date), $changeType, $path"
    foreach ($subPath in @("code\ui\", "models\", "materials\")) {
        if ($path.StartsWith("$(Get-Location)\$($subPath)")) {
            $smallPath = ""
            try {
                push-location "$(Get-Location)\$($subPath)"
                $smallPath = Resolve-Path -relative $path
            } finally {
                Pop-Location
            }
            $oldPath = "$($subPath)$($smallPath)"
            $newPath = "..\..\..\$($subPath)$($smallPath)"
            Write-Host "Copying $($oldPath) to $($newPath)"
            New-Item -Type dir (split-path $newPath -Parent)
            Copy-Item $oldPath $newPath
        }
    }
}

$handlers = . {
    Register-ObjectEvent -InputObject $watcher -EventName Changed  -Action $action
    Register-ObjectEvent -InputObject $watcher -EventName Created  -Action $action
    Register-ObjectEvent -InputObject $watcher -EventName Deleted  -Action $action
    Register-ObjectEvent -InputObject $watcher -EventName Renamed  -Action $action
}
$watcher.EnableRaisingEvents = $true
Write-Host
Write-Host "Watching for changes to asset directories in $(Get-Location)..."


try {
  do {
    # Wait-Event waits for a second and stays responsive to events
    Wait-Event -Timeout 1
  } while ($true)
} finally {
  $watcher.EnableRaisingEvents = $false

  $handlers | ForEach-Object {
    Unregister-Event -SourceIdentifier $_.Name
  }
  $handlers | Remove-Job

  $watcher.Dispose()
}
