profiles_path="/home/lucius/.local/share/gale/lethal-company/profiles"
mod_profile="MultiInstanceTest"
mod_author="LuciusofLegend"
mod_name="QuotaOverhaul"
debug_folder="./src/$mod_name/bin/Debug"

dotnet build
cp "./CHANGELOG.md" "./artifacts/tspublish/icon.png" "./LICENSE.md" "$debug_folder/$mod_name.deps.json" "$debug_folder/$mod_name.dll" "$profiles_path/$mod_profile/BepInEx/plugins/$mod_author-$mod_name"