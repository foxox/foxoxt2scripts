import pathlib
import zipfile
import os

# This script helps you develop a Tribes 2 vl2 package outside of your GameData folder, to keep things tidy.
# For example, make a folder on your desktop and put your content in there (script, skins, whatever).
# Put this script in there too. Run this script to pack up all your stuff as a vl2 file and move it over to GameData.

# In more detail, your desktop folder might be like:
# desktop/my_cool_skin
# desktop/my_cool_skin/deploy.py
# desktop/my_cool_skin/textures
# desktop/my_cool_skin/textures/skins
# desktop/my_cool_skin/textures/skins/coolskin.hmale.png
# desktop/my_cool_skin/textures/skins/coolskin.lmale.png
# desktop/my_cool_skin/textures/skins/coolskin.mmale.png

# Make a change to your skin, then run "python deploy.py".
# It will pack up your skin as a vl2 and put it in your GameData folder.
# It will replace any existing one with the same name.

# The directory of stuff to pack into the vl2 file.
# This directory should have your "scripts", "textures", etc. folders in it.
# If you put this deploy.py script in the same place, you can leave this as "./"
directory = pathlib.Path("./")

# The output vl2 filename
output_vl2_name = "MyFlagStatus20251001a.vl2"

# Your GameData\base directory.
output_directory = r'C:\XProgramFiles\Sierra\2025preconfigured\GameData\base'



# From here down is script stuff:

output_path_and_file_joined = os.path.join(output_directory,output_vl2_name)

print('Deploying content from \"' + str(directory) + '\" to \"' + output_path_and_file_joined + '\"...')

if os.path.isfile(output_path_and_file_joined):
  os.remove(output_path_and_file_joined)

with zipfile.ZipFile(output_vl2_name, mode="w") as archive:
  for file_path in directory.rglob("*"):
    archive.write(file_path, arcname=file_path.relative_to(directory))

os.rename(output_vl2_name, output_path_and_file_joined)

print('Done.')