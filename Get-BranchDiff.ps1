<#
.SYNOPSIS
Shows differences between current branch and its source branch

.DESCRIPTION
Uses git reflog to find the branch from which the current branch was created,
then shows a diff between the two branches.
#>

# Get current branch name
$currentBranch = git branch --show-current
if (-not $currentBranch) {
    Write-Error "Not in a git repository"
    exit 1
}

# Execute the exact command that works for you
$reflogOutput = git reflog --date=iso | 
    Select-String -Pattern "checkout: moving from" -Context 0,1 |
    Where-Object { $_ -match "to $currentBranch" }

if (-not $reflogOutput) {
    Write-Error "Could not determine source branch for '$currentBranch'"
    exit 2
}

# Extract source branch name from the actual matching line
$sourceBranch = $reflogOutput.Line -replace '.*moving from (\S+) to .*', '$1'

Write-Host "Current branch: $currentBranch" -ForegroundColor Cyan
Write-Host "Source branch:  $sourceBranch" -ForegroundColor Cyan
Write-Host "`nDifferences between $sourceBranch and $currentBranch`n" -ForegroundColor Yellow

# Show branch differences
git diff "${currentBranch}..${sourceBranch}"
