#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Manages custom commands in PowerShell profile.

.DESCRIPTION
    This script adds or removes a set of custom commands and aliases to/from the PowerShell profile file.
    The commands are wrapped in a marker block for easy identification and removal.

.PARAMETER Remove
    Remove the custom commands from the profile instead of adding them.

.PARAMETER Force
    Force the operation without prompting for confirmation.

.EXAMPLE
    .\Manage-ProfileCommands.ps1
    Adds custom commands to the PowerShell profile.

.EXAMPLE
    .\Manage-ProfileCommands.ps1 -Remove
    Removes custom commands from the PowerShell profile.

.EXAMPLE
    .\Manage-ProfileCommands.ps1 -Force
    Adds custom commands without confirmation prompt.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$false)]
    [switch]$Remove,
    
    [Parameter(Mandatory=$false)]
    [switch]$Force
)

# Marker to identify the managed block in the profile
$StartMarker = "# ===== BEGIN: KubePizza Custom Commands ====="
$EndMarker = "# ===== END: KubePizza Custom Commands ====="

# Get the directory where this script is located
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Array of aliases (Name -> Relative Path from script directory)
$AliasesRelativePaths = @{
    "kp01" = "01-Basic\bin\Debug\net10.0\kp01.exe"
    "kp02" = "02-SubCommands\bin\Debug\net10.0\kp02.exe"
    "kp03" = "03-Validators\bin\Debug\net10.0\kp03.exe"
    "kp04" = "04-TabCompletion\bin\Debug\net10.0\kp04.exe"
    "kp05" = "05-CustomHelp\bin\Debug\net10.0\kp05.exe"
    "kp" = "06-SolutionWithTests\KubePizza.Console\bin\Debug\net10.0\kp.exe"
}

# Build full paths for aliases
$Aliases = @{}
foreach ($alias in $AliasesRelativePaths.GetEnumerator()) {
    $fullPath = Join-Path -Path $ScriptDir -ChildPath $alias.Value
    $Aliases[$alias.Key] = $fullPath
}

# Build the commands block dynamically
$commandsBlock = New-Object System.Text.StringBuilder
[void]$commandsBlock.AppendLine($StartMarker)
[void]$commandsBlock.AppendLine()

# Add aliases
if ($Aliases.Count -gt 0) {
    [void]$commandsBlock.AppendLine("# Custom aliases")
    foreach ($alias in $Aliases.GetEnumerator()) {
        [void]$commandsBlock.AppendLine("Set-Alias -Name $($alias.Key) -Value $($alias.Value) -Option AllScope -ErrorAction SilentlyContinue")
    }
    [void]$commandsBlock.AppendLine()
}

[void]$commandsBlock.Append($EndMarker)

$CommandsToAdd = $commandsBlock.ToString()

# Function to check if commands are already in profile
function Test-CommandsInProfile {
    param([string]$ProfilePath)
    
    if (-not (Test-Path $ProfilePath)) {
        return $false
    }
    
    $content = Get-Content $ProfilePath -Raw
    return $content -match [regex]::Escape($StartMarker)
}

# Function to add commands to profile
function Add-CommandsToProfile {
    param([string]$ProfilePath)
    
    # Check if profile exists, if not create it
    if (-not (Test-Path $ProfilePath)) {
        Write-Host "Profile file not found. Creating: $ProfilePath" -ForegroundColor Yellow
        New-Item -Path $ProfilePath -ItemType File -Force | Out-Null
    }
    
    # Check if commands already exist
    if (Test-CommandsInProfile -ProfilePath $ProfilePath) {
        Write-Host "Commands already exist in profile. Use -Remove to remove them first." -ForegroundColor Yellow
        return $false
    }
    
    # Add commands to profile
    Add-Content -Path $ProfilePath -Value "`n$CommandsToAdd`n"
    Write-Host "? Custom commands added successfully to profile!" -ForegroundColor Green
    Write-Host "  Profile location: $ProfilePath" -ForegroundColor Gray
    
    # Display added aliases
    if ($Aliases.Count -gt 0) {
        Write-Host "`nAdded aliases:" -ForegroundColor Cyan
        foreach ($alias in $Aliases.GetEnumerator()) {
            Write-Host "  - $($alias.Key) -> $($alias.Value)" -ForegroundColor White
        }
    }
    
    Write-Host "`nRestart PowerShell or run: . `$PROFILE" -ForegroundColor Yellow
    
    return $true
}

# Function to remove commands from profile
function Remove-CommandsFromProfile {
    param([string]$ProfilePath)
    
    if (-not (Test-Path $ProfilePath)) {
        Write-Host "Profile file not found: $ProfilePath" -ForegroundColor Yellow
        return $false
    }
    
    if (-not (Test-CommandsInProfile -ProfilePath $ProfilePath)) {
        Write-Host "Commands not found in profile. Nothing to remove." -ForegroundColor Yellow
        return $false
    }
    
    # Read the profile content
    $content = Get-Content $ProfilePath -Raw
    
    # Remove the managed block using regex with Singleline option to match across newlines
    $pattern = [regex]::Escape($StartMarker) + "[\s\S]*?" + [regex]::Escape($EndMarker)
    $newContent = [regex]::Replace($content, $pattern, "", [System.Text.RegularExpressions.RegexOptions]::Singleline)
    
    # Remove extra blank lines (3 or more consecutive newlines to 2)
    $newContent = [regex]::Replace($newContent, "(\r?\n){3,}", "`r`n`r`n")
    
    # Trim and write back to profile
    $trimmedContent = $newContent.Trim()
    
    if ([string]::IsNullOrWhiteSpace($trimmedContent)) {
        # If the profile is empty after removal, delete it or write empty content
        Set-Content -Path $ProfilePath -Value "" -NoNewline
    } else {
        Set-Content -Path $ProfilePath -Value $trimmedContent -NoNewline
    }

    Write-Host "? Custom commands removed successfully from profile!" -ForegroundColor Green
    Write-Host "  Profile location: $ProfilePath" -ForegroundColor Gray
    Write-Host "`nRestart PowerShell or run: . `$PROFILE" -ForegroundColor Yellow
    
    return $true
}

# Main script execution
try {
    $profilePath = $PROFILE
    
    Write-Host "`n=== PowerShell Profile Manager ===" -ForegroundColor Cyan
    Write-Host "Profile path: $profilePath`n" -ForegroundColor Gray
    
    if ($Remove) {
        # Remove commands
        if (-not $Force) {
            $confirmation = Read-Host "Remove custom commands from profile? (Y/N)"
            if ($confirmation -ne 'Y' -and $confirmation -ne 'y') {
                Write-Host "Operation cancelled." -ForegroundColor Yellow
                exit 0
            }
        }
        
        $result = Remove-CommandsFromProfile -ProfilePath $profilePath
        exit $(if ($result) { 0 } else { 1 })
    }
    else {
        # Add commands
        if (-not $Force) {
            Write-Host "This will add custom commands to your PowerShell profile." -ForegroundColor Yellow
            $confirmation = Read-Host "Continue? (Y/N)"
            if ($confirmation -ne 'Y' -and $confirmation -ne 'y') {
                Write-Host "Operation cancelled." -ForegroundColor Yellow
                exit 0
            }
        }
        
        $result = Add-CommandsToProfile -ProfilePath $profilePath
        exit $(if ($result) { 0 } else { 1 })
    }
}
catch {
    Write-Host "`n? Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
