import numpy as np

# Observer Camera Format:
# new Camera()
# {
#   position = "-482.862 327.444 163.649";
#   rotation = "0.373651 0.211465 -0.903143 64.1455";
#   scale = "1 1 1";
#   dataBlock = "Observer";
#   lockCount = "0";
#   homingCount = "0";
#   locked = "true";
# };


#  Dark Tiger says set a keybind for this to make observer cameras easier in the level editor
# package camS{
#    function OptionsDlg::onWake(%this) {
#       if(!$camDrop) {
# 		   $RemapName[$RemapCount]="\c4Add Camera Drop Point ";
#          $RemapCmd[$RemapCount]="addCamDrop";
# 		   $RemapCount++;

pos_center = [-558.346, 608.073, 162.857]
pos_right = [-587.538, 526.664, 162.857]

num_points = 45 # at least 2

slide_scale = 0.5
# If at closest focus, the focus point is too far away, try reducing the scale.


delta_center_right = np.multiply(np.add(pos_right,np.negative(pos_center)), slide_scale)
pos_slide_start = np.add(pos_center,np.negative(delta_center_right))

num_increments = num_points-1
incremental_delta = np.multiply(delta_center_right, 2) / num_increments
for i in range(0,num_points):
  pos = np.add(pos_slide_start, np.multiply(incremental_delta,i))
  # print(i,'pos:',pos)
  print( \
    'new Camera()\n' \
    '{\n' \
    'position = "',pos[0], pos[1], pos[2],'";\n' \
    'rotation = "0 0 1 110.008";\n' \
    'scale = "1 1 1";\n' \
    'dataBlock = "Observer";\n' \
    'lockCount = "0";\n' \
    'homingCount = "0";\n' \
    '};'
    )