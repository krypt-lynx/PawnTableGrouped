
# 1414302321 Mehni.Numbers                        Numbers
# 2144935009 syl.simpleslavery                    Simple Slavery [1.2]
# 2345493945 DerekBickley.LTOColonyGroupsFinal    [LTO] Colony Groups
#  725219116 fluffy.worktab                       Work Tab
#  753498552 Orion.Hospitality                    Hospitality
# 2009463077 brrainz.harmony                      Harmony

# 2209393954 name.krypt.rimworld.moddiff          RWLayout
# add this id to the modIds list if you are missing local copy of RWLayout

$appId = 294100
$modIds = @(1414302321, 2144935009, 2345493945, 725219116, 753498552, 2009463077)
$appVersions = @("1.1", "1.2", "1.3", "1.4")

[string]$startupPath = Get-Location
$7z = (GET-ItemProperty 'HKLM:\SOFTWARE\7-Zip').Path + '7z.exe'
$cache = $startupPath + "\cache"


$steamcmd_location = $cache + "\steamcmd"
$steamcmd = $steamcmd_location +"\steamcmd.exe"

New-Item -Force -Path $cache -ItemType Directory | Out-Null

if (-Not (Test-Path $steamcmd)) {
	# download SteamCMD
	Echo "Downloading steam console client..."
	
	$steamcmd_url = "http://media.steampowered.com/installer/steamcmd.zip"
	$steamcmd_archPath = $cache + "\steamcmd.zip"

	Invoke-WebRequest -Uri $steamcmd_url -OutFile $steamcmd_archPath
	& $7z e $steamcmd_archPath "-o$steamcmd_location"
	Remove-Item -Recurse -Force $steamcmd_archPath
}


Echo "Downloading mods..."

$steamCmdArgs = @("+force_install_dir $cache", "+login anonymous")
$steamCmdArgs += foreach ($modId in $modIds) {
	"+workshop_download_item $appId $modId"
}
$steamCmdArgs += "+quit"
& $steamcmd $steamCmdArgs 


$modsDownloads = $cache + "\steamapps\workshop\content\" + $appId
Echo $modsDownloads

$assemplyPathResolver = $startupPath + "\ResolveModLoadPaths.exe"

Echo "Coping assemblies..."

foreach ($ver in $appVersions) {
	$dependencies = $startupPath + "\" + $ver
	Remove-Item -Recurse -Force $dependencies | Out-Null	
	New-Item -Force -Path $dependencies -ItemType Directory | Out-Null
	
	foreach ($modId in $modIds) {
		& $assemplyPathResolver --path "$modsDownloads\$modId" --version $ver | Foreach-Object -Process {
			Copy-Item -Recurse $_ -Destination $dependencies
		}
	}	
}

Echo "Done!"