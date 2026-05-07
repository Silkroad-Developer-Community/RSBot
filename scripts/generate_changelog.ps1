param(
    [Parameter(Mandatory=$true)]
    [string]$BaseSha,
    [string]$OutputFile = "commit_log.md"
)

# Force UTF-8 encoding for standard output and PowerShell 7+
$OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "Generating changelog since $BaseSha..."

# 1. Get commit list from git locally
$gitCommand = "git log ""$BaseSha..HEAD"" --pretty=""format:%H|%s"""
$commitsRaw = Invoke-Expression $gitCommand
if (-not $commitsRaw) {
    Write-Warning "No commits found in the specified range."
    exit 0
}

# Convert to objects
$commits = $commitsRaw | ConvertFrom-Csv -Delimiter '|' -Header SHA, Subject

# 2. Bulk fetch metadata from GitHub API to get logins
# gh api --paginate returns a stream of JSON objects (pages)
Write-Host "Fetching GitHub metadata..."
try {
    # We use -L to follow redirects and -q to filter if needed, 
    # but here we just get all commits to map SHAs.
    $apiJson = gh api "repos/Silkroad-Developer-Community/RSBot/commits?per_page=100" --paginate
    $apiCommits = $apiJson | ConvertFrom-Json
} catch {
    Write-Error "Failed to fetch data from GitHub API. Ensure GITHUB_TOKEN is set."
    $apiCommits = @()
}

# 3. Create mapping and build log
$log = @("## List of changes")

foreach ($c in $commits) {
    # Find matching commit in API results
    $apiMatch = $apiCommits | Where-Object { $_.sha -eq $c.SHA }
    
    $authorDisplay = ""
    if ($apiMatch -and $apiMatch.author -and $apiMatch.author.login) {
        $authorDisplay = "@" + $apiMatch.author.login
    } else {
        # Fallback to local git name if API resolution fails
        $localName = git log -n 1 $c.SHA --format='%an'
        $authorDisplay = $localName.Trim()
    }

    $log += "- [$($c.Subject)](https://github.com/Silkroad-Developer-Community/OasisBot/commit/$($c.SHA)) - $authorDisplay"
}

# 4. Save with UTF8 encoding
$log | Set-Content -Path $OutputFile -Encoding UTF8
Write-Host "Changelog generated: $OutputFile"

