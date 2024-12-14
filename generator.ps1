# Parameters
param(
    [int]$FileSizeMB = 1, # Desired file size in MB
    [string]$OutputPath = "data.txt" # Output file path
)

# Function to generate a random 8-digit ID
function Generate-RandomId {
    return "{0:D8}" -f (Get-Random -Minimum 1 -Maximum 100000000)
}

# Function to generate random ISO 8601 timestamps (formatted as yyyy-MM-ddTHH:mm:ss)
function Generate-RandomTimestamps {
    $startDate = Get-Date -Year 2024 -Month 1 -Day 1
    $endDate = Get-Date -Year 2024 -Month 12 -Day 31
    $startTime = $startDate.AddSeconds((Get-Random -Minimum 0 -Maximum ($endDate - $startDate).TotalSeconds))

    # Ensure end time is always later than start time (random range of 1 minute to 5 hours)
    $durationInSeconds = Get-Random -Minimum 60 -Maximum 18000
    $endTime = $startTime.AddSeconds($durationInSeconds)

    # Return start and end times in the desired format
    return @($startTime.ToString("yyyy-MM-ddTHH:mm:ss"), $endTime.ToString("yyyy-MM-ddTHH:mm:ss"))
}

# Generate Data
$lineSizeBytes = 64 # Approximate size of one line (adjust if needed)
$linesCount = [math]::Ceiling(($FileSizeMB * 1MB) / $lineSizeBytes)

Write-Host "Generating $linesCount lines (~$FileSizeMB MB)..."

$stream = [System.IO.StreamWriter]::new($OutputPath)
try {
    for ($i = 1; $i -le $linesCount; $i++) {
        $timestamps = Generate-RandomTimestamps
        $id = Generate-RandomId
        $line = "$($timestamps[0]) $($timestamps[1]) $id"
        $stream.WriteLine($line)

        # Progress indicator
        if ($i % 1000 -eq 0) {
            Write-Host "$i lines generated..." -NoNewline
            Start-Sleep -Milliseconds 100
            Write-Host "`r" -NoNewline
        }
    }
    Write-Host "File generation complete. Saved to $OutputPath."
} finally {
    $stream.Close()
}
