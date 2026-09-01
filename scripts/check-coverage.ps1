param([string] $ResultsPath = "TestResults", [double] $Minimum = 80)
$ErrorActionPreference = "Stop"
$reports = Get-ChildItem -LiteralPath $ResultsPath -Recurse -Filter "coverage.cobertura.xml"
if ($reports.Count -eq 0) { throw "Relatório de cobertura não encontrado." }
$lines = @{}
foreach ($report in $reports) {
    [xml] $document = Get-Content -LiteralPath $report.FullName
    foreach ($class in $document.coverage.packages.package.classes.class) {
        $file = ([string] $class.filename).Replace("\", "/")
        if ($file -match "/obj/|/Migrations/|/Program\.cs$|/DependencyInjection\.cs$") { continue }
        foreach ($line in $class.lines.line) {
            $key = "$file`:$($line.number)"; $hits = [int] $line.hits
            if (-not $lines.ContainsKey($key) -or $hits -gt $lines[$key]) { $lines[$key] = $hits }
        }
    }
}
$covered = @($lines.Values | Where-Object { $_ -gt 0 }).Count
$percentage = [Math]::Round(($covered / $lines.Count) * 100, 2)
Write-Output "Cobertura consolidada: $percentage% ($covered/$($lines.Count))."
if ($percentage -lt $Minimum) { throw "Cobertura abaixo de $Minimum%." }
