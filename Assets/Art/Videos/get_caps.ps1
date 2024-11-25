[CmdletBinding()]
param(
    [switch]$power_of_two = $false
)

$oldvids = Get-ChildItem *.mp4 -Recurse

if ($power_of_two){
	foreach ($oldvid in $oldvids) {
		$cap = [io.path]::ChangeExtension($oldvid.FullName, '.png')
		ffmpeg -ss 00:00:05 -i $oldvid.FullName -y -vf "scale=2048:1024:force_original_aspect_ratio=decrease,pad=2048:1024:(ow-iw)/2:(oh-ih)/2" -update true -frames:v 1 -q:v 1 $cap
	}
}
else{
	foreach ($oldvid in $oldvids) {
		$cap = [io.path]::ChangeExtension($oldvid.FullName, '.png')
		ffmpeg -ss 00:00:05 -i $oldvid.FullName -y -update true -frames:v 1 -q:v 1 $cap
	}
}