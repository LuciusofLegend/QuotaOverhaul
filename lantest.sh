profiles_folder="~/.local/share/gale/lethal-company/profiles"
mod_profile="MultiInstanceTest"

lethal_exe="/home/lucius/.steam/steam/steamapps/common/Lethal Company/Lethal Company.exe"
proton_path="/home/lucius/.local/share/Steam/compatibilitytools.d/GE-Proton9-27"
lethal_wineprefix="/home/lucius/.local/share/Steam/steamapps/compatdata/1966720"

cd "$profiles_folder/$mod_profile"

export PROTON_VERB=runinprefix
export PROTONPATH="$proton_path"
export WINEPREFIX="$lethal_wineprefix"
export WINEDLLOVERRIDES="winhttp.dll=n"

#umu-run "$lethal_exe" --doorstop-enable true --doorstop-target "$profiles_folder/$mod_profile/BepInEx/core/BepInEx.Preloader.dll"
"/usr/bin/steam" -applaunch "1966720" --doorstop-enable true --doorstop-target "$profiles_path/$mod_profile/BepInEx/core/BepInEx.Preloader.dll"