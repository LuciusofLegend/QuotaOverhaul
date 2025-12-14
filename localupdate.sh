profiles_path="/home/lucius/.local/share/com.kesomannen.gale/lethal-company/profiles"
mod_profile="QuotaOverhaulTesting"
mod_author="LuciusofLegend"
mod_name="QuotaOverhaul"
output_folder="./src/$mod_name/bin/Debug"

cp "./CHANGELOG.md" "./artifacts/tspublish/icon.png" "./LICENSE.md" "$output_folder/$mod_name.deps.json" "$output_folder/$mod_name.dll" "$profiles_path/$mod_profile/BepInEx/plugins/$mod_author-$mod_name"