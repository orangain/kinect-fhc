param(
	[string]$destFile,
	[string]$srcDir
)

[Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem")

function zip([string]$destFile, [string]$srcDir)
{
	$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
	$includeBaseDir = $false
	[System.IO.Compression.ZipFile]::CreateFromDirectory($srcDir, $destFile, $compressionLevel, $includeBaseDir)
}

if (Test-Path $destFile)
{
	rm $destFile
}

zip "$destFile" "$srcDir"
