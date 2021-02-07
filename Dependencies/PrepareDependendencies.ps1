
# 1414302321 Mehni.Numbers                        Numbers
# 2144935009 syl.simpleslavery                    Simple Slavery [1.2]
# 2345493945 DerekBickley.LTOColonyGroupsFinal    [LTO] Colony Groups
#  725219116 fluffy.worktab                       Work Tab
#  753498552 Orion.Hospitality                    Hospitality
# 2009463077 brrainz.harmony                      Harmony

$appId = 294100
$modIds = @(1414302321, 2144935009, 2345493945, 725219116, 753498552, 2009463077)


[string]$startupPath = Get-Location
$vswhere = "$Env:programfiles (x86)\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuild = & $vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | select-object -first 1
$7z = (GET-ItemProperty 'HKLM:\SOFTWARE\7-Zip').Path + '7z.exe'
$rw = (GET-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 294100').InstallLocation

$modsLocal = $rw + '\Mods\' + $internalPath
$cache = $startupPath + "\cache"
$dependencies = $startupPath
$dependencies_1_1 = $startupPath + "\1.1"
$dependencies_1_2 = $startupPath + "\1.2"


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

$steamCmdArgs = @("+login anonymous", "+force_install_dir $cache")
$steamCmdArgs += foreach ($modId in $modIds) {
	"+workshop_download_item $appId $modId"
}
$steamCmdArgs += "+quit"
& $steamcmd $steamCmdArgs 


$modsDownloads = $cache + "\steamapps\workshop\content\" + $appId
Echo $modsDownloads

$assemplyPathResolver = $startupPath + "\ResolveModLoadPaths.exe"

Echo "Coping assemblies..."


Remove-Item -Recurse -Force $dependencies_1_1
Remove-Item -Recurse -Force $dependencies_1_2

New-Item -Force -Path $dependencies -ItemType Directory | Out-Null
New-Item -Force -Path $dependencies_1_1 -ItemType Directory | Out-Null
New-Item -Force -Path $dependencies_1_2 -ItemType Directory | Out-Null

foreach ($modId in $modIds) {
	& $assemplyPathResolver --path "$modsDownloads\$modId" --version "1.1" | Foreach-Object -Process {
		Copy-Item -Recurse $_ -Destination $dependencies_1_1
	}
}


foreach ($modId in $modIds) {
	& $assemplyPathResolver --path "$modsDownloads\$modId" --version "1.2" | Foreach-Object -Process {
		Copy-Item -Recurse $_ -Destination $dependencies_1_2
	}
}


Echo "Done!"