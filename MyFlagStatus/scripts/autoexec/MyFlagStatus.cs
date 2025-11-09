// #autoload
// #name = MyFlagStatus
// #version = 1.2
// #date = 14 February, 2003
// #author = foxox
// #description = Icon for the GUI which changes when you take the flag.
// #category = Foxox's Scripts
// #status = release
// #include = support/callback.cs
// #include = support/flag_tracker.cs
// #include = support/player_support.cs
// #include = support/mission_callbacks.cs
// #config = MyFlagStatusGui


// Based on a script by ilys

// Script config:

/// Here you can configure the icons displayed based on the flag state.
/// If you wish to disable one mode, simply change it to "gui/fully_transparent.png" to make it invisible.

/// The file to display when the player holds the flag.
$flag_icon_held = "gui/icon_flag_indicator.png";

/// The file to display when the player's teammate holds the flag.
$flag_icon_held_by_team = "gui/held_team.png";

/// The file to display when the enemy flag is on the enemy flagstand.
$flag_icon_on_stand = "gui/on_stand.png";

/// The file to display when the enemy flag is loose in the field.
$flag_icon_in_field = "gui/in_field.png";

/// The size of the icon on the screen. It should be square (both numbers the same) for a square input texture.
$gui_item_extent = "50 50";


// Below here is script logic stuff that controls how the script works.


/// MyFlagStatus package. This bundles all of the parts of this script together.
package MyFlagStatus {

/// Some helper functions added by foxox in order to deal with the 2025 Tribes Next patch

/// Gets the screen resolution, but scaled based on the UI Scale setting
/// @param[in] index Indicates whether you want horizontal or vertical. 0 = horizontal resolution, 1 = vertical resolution
function getUIScaledResolution(%index)
{
  return getword($pref::Video::resolution, %index) / getUIScale();
  //$pref::Video::uiScale;
}

/// Gets the UI Scaled horizontal screen center coordinate
function getUIScaledHorizontalCenter()
{
  return mFloor(getUIScaledResolution(0) / 2);
}

/// Gets the UI Scaled vertical screen center coordinate
function getUIScaledVerticalCenter()
{
  return mFloor(getUIScaledResolution(1) / 2);
}

/// Echos stuff to the console, but only when debugging is enabled.
/// @param[in] text The text to output to the console via echo().
function debuglog(%text)
{
  // Enable or disable debuglog here:
  %debug_mode = false;
  if (%debug_mode)
  {
    echo(%text);
  }
}

/// When the game UI starts up, set up this script's GUI item.
function PlayGui::onWake(%this)
{
  debuglog("onWake");

  // Set up the flag-related callbacks. These functions implemented below are invoked when the corresponding event happens.
	callback.add(onCTFGrab, FlagStatusFlagTaken);
	callback.add(onCTFCap, FlagStatusFlagReturn);
	callback.add(onCTFDrop, FlagStatusFlagDrop);
	callback.add(onCTFPicked, FlagStatusFlagTaken);
	callback.add(onCTFReturn, FlagStatusFlagReturn);
	callback.add(onMissionEnd, FlagStatusMissionEnd);
	callback.add(onGameOver, FlagStatusMissionEnd);
	callback.add(MyTeamChanged, FlagStatusTeamChange);
	callback.add(onUserClientDrop, FlagStatusDrop);
	parent::onWake(%this);

  // If there is no game UI, exit. There must be a game UI to which to add the new element.
	if(!isObject(PlayGui))
  {
    return;
  }

  // If the GUI item already exists, remove it before adding it again. This should let you reload the script multiple times for debugging or whatever.
	if (isObject(EnemyFlagStatus))
  {
    PlayGui.remove(EnemyFlagStatus);
  }

  // Create the little icon that appears for this script on the screen
	new GuiBitmapCtrl(EnemyFlagStatus) {
		horizSizing = "right";
		vertSizing = "bottom";
    // Set the GUI item position here:
		position = (getUIScaledHorizontalCenter() - 150) SPC getUIScaledVerticalCenter();
    // Set the GUI item size here:
		extent = $gui_item_extent;
		minExtent = "16 16";
		visible = "0"; // start it out invisible, it will appear when joining a team from observer.
		bitmap = $flag_icon_on_stand;
	};
  // And add it to the game UI so the player sees it
	playGui.add(EnemyFlagStatus);

	if(isObject(HM) && isObject(HudMover))
	{
		hudmover::addhud(EnemyFlagStatus,"FlagStatusEnemy");
	}
}

/// Runs when the mission ends
function FlagStatusMissionEnd()
{
  debuglog("mission end");
	EnemyFlagStatus.setbitmap($flag_icon_on_stand);
}

/// Runs when connection is dropped
/// @param[in] clientName The client name
/// @param[in] clientID The client ID
function FlagStatusDrop(%clientName, %clientID)
{
  debuglog("mission drop");
	if (isObject(EnemyFlagStatus))
	{
		playGui.remove(EnemyFlagStatus);
		EnemyFlagStatus.delete();
	}
}

/// Runs when a flag is dropped
/// @param[in] flagref Information about the flag
function FlagStatusFlagDrop(%flagref)
{
  debuglog("drop");
	%myname = PlayerList.getMyName();
	%myteam = PlayerList.getTeamByName(%myname);
	%flagteam = %flagref.teamID;
	if(%flagteam != %myteam && %myteam != 0)
	{
		EnemyFlagStatus.setbitmap($flag_icon_in_field);
	}
}

/// Runs when a flag is taken
/// @param[in] flagref Information about the flag
function FlagStatusFlagTaken(%flagref)
{
  debuglog("taken");
	%myname = PlayerList.getMyName();
  %myname_stripped = stripMLControlChars(%myname);
	%myteam = PlayerList.getTeamByName(%myname);
	%flagteam = %flagref.teamID;
  %flag_holder_name = baseName(%flagref.actorCurrent);
  
  debuglog("myteam" SPC %myteam);
  debuglog("flagteam" SPC %flagteam);
  debuglog("myname >" @ %myname @ "<" SPC strLen(%myname));
  debuglog("myname_stripped >" @ %myname_stripped @ "<" SPC strLen(%myname_stripped));
  debuglog("flag_holder_name >" @ %flag_holder_name @ "<" SPC strLen(%flag_holder_name));

	if(%flagteam != %myteam && %myteam != 0)
  {
    if (%myname_stripped $= %flag_holder_name)
    {
      debuglog("taken check passed me");
      EnemyFlagStatus.setbitmap($flag_icon_held);
    }
    else
    {
      debuglog("taken check passed teammate");
      EnemyFlagStatus.setbitmap($flag_icon_held_by_team);
    }
  }
}

/// Runs when a flag is returned
/// @param[in] flagref Information about the flag
function FlagStatusFlagReturn(%flagref)
{
  debuglog("return");
	%myname = PlayerList.getMyName();
	%myteam = PlayerList.getTeamByName(%myname);
	%flagteam = %flagref.teamID;
	if(%flagteam != %myteam && %myteam != 0)
	{
		EnemyFlagStatus.setbitmap($flag_icon_on_stand);
	}
}

/// Runs when team is changed
/// @param[in] teamID ID of the team changed to
function FlagStatusTeamChange(%teamID)
{
  debuglog("team change");

	EnemyFlagStatus.setbitmap($flag_icon_on_stand);

  // Enbale the icon when switching from observer to an actual team
	if(%teamID != 0)
	{
		EnemyFlagStatus.setvisible("1");

		%myteam = %teamID;
		if(%myteam == 1)
    {
      %enemyteam = 2;
    }
		else
    {
			%enemyteam = 1;
    }

		debuglog("%myteam" SPC %myteam);
		debuglog("%enemyteam" SPC %enemyteam);

		%myflag = FlagTracker.team[%myteam];
		%enemyflag = FlagTracker.team[%enemyteam];

    if(%enemyflag.stateCurrent $= "Taken")
		{
			EnemyFlagStatus.setbitmap($flag_icon_held);
		}
		else if(%enemyflag.stateCurrent $= "In Field")
		{
			circleenemycolorswapcycle();
		}
	}
	else // Disable the icon when switching to observer
	{
		EnemyFlagStatus.setvisible("0");
	}
}

function MyFlagStatusIconSizeSlider::onSelect(%this)
{
  debuglog("my flag status icon size slider onSelect");
}

function EnableOnStandIconCtrl::onSelect(%this)
{
  debuglog("enable on stand icon ctrl onSelect");
}

function setup(%this)
{
  exec("prefs/clientprefs.cs");

  // If one of the prefs doesn't exist, assume they all don't exist, so set them all.
  if($pref::MyFlagStatus::EnableOnStandIcon $= "")
  {
    $pref::MyFlagStatus::EnableOnStandIcon = true;
    $pref::MyFlagStatus::EnableInFieldIcon = true;
    $pref::MyFlagStatus::EnableHeldIcon = true;
    $pref::MyFlagStatus::EnableHeldTeamIcon = true; 
  }

  new ShellFieldCtrl(MyFlagStatusGui)
  {
    profile = "GuiChatBackProfile";
    horizSizing = "center";
    vertSizing = "center";
    position = "31 25";
    extent = "337 302";
    minExtent = "337 253";
    visible = "1";

    new GuiButtonBaseCtrl()
    {
      profile = "ShellHoloBox";
      horizSizing = "center";
      vertSizing = "top";
      position = "6 5";
      extent = "324 147";
      minExtent = "8 8";
      visible = "1";
      groupNum = "-1";
      buttonType = "PushButton";

        new GuiTextCtrl()
      {
        profile = "TR2BonusBigText";
        horizSizing = "right";
        vertSizing = "bottom";
        position = "99 3";
        extent = "138 22";
        minExtent = "8 8";
        visible = "1";
        text = "My Flag Status";
        longTextBuffer = "0";
        maxLength = "255";
      };
    };

    new GuiTextCtrl(MyFlagStatusIconSizeText)
    {
      profile = "GuiTextObjGreenCenterProfile";
      horizSizing = "right";
      vertSizing = "bottom";
      position = "13 91";
      extent = "133 19";
      minExtent = "8 8";
      visible = "1";
      text = "Flag Status Icon Size:";
      longTextBuffer = "0";
      maxLength = "255";
    };

    new ShellSliderCtrl(MyFlagStatusIconSizeSlider)
    {
      profile = "ShellSliderProfile";
      horizSizing = "right";
      vertSizing = "bottom";
      position = "188 107";
      extent = "117 24";
      minExtent = "12 24";
      visible = "1";
      variable = "value";
      altCommand = "FrameTransSlider.onSelect();";
      range = "0.000000 100.000000";
      ticks = "0";
      value = "50";
      usePlusMinus = "0";
    };

    new ShellToggleButton(EnableOnStandIconCtrl)
    {
      profile = "ShellRadioProfile";
      horizSizing = "right";
      vertSizing = "bottom";
      position = "45 33";
      extent = "155 30";
      minExtent = "26 27";
      visible = "1";
      hideCursor = "0";
      bypassHideCursor = "0";
      variable = "$pref::MyFlagStatus::EnableOnStandIcon";
      helpTag = "0";
      text = "Enable Flag On Stand Icon";
      longTextBuffer = "0";
      maxLength = "255";
    };

    new ShellToggleButton(EnableInFieldIconCtrl)
    {
      profile = "ShellRadioProfile";
      horizSizing = "right";
      vertSizing = "bottom";
      position = "45 33";
      extent = "155 30";
      minExtent = "26 27";
      visible = "1";
      hideCursor = "0";
      bypassHideCursor = "0";
      variable = "$pref::MyFlagStatus::EnableInFieldIcon";
      helpTag = "0";
      text = "Enable Flag On Stand Icon";
      longTextBuffer = "0";
      maxLength = "255";
    };

    new ShellToggleButton(EnableHeldIconCtrl)
    {
      profile = "ShellRadioProfile";
      horizSizing = "right";
      vertSizing = "bottom";
      position = "45 33";
      extent = "155 30";
      minExtent = "26 27";
      visible = "1";
      hideCursor = "0";
      bypassHideCursor = "0";
      variable = "$pref::MyFlagStatus::EnableHeldIcon";
      helpTag = "0";
      text = "Enable Flag On Stand Icon";
      longTextBuffer = "0";
      maxLength = "255";
    };

    new ShellToggleButton(EnableHeldTeamIconCtrl)
    {
      profile = "ShellRadioProfile";
      horizSizing = "right";
      vertSizing = "bottom";
      position = "45 33";
      extent = "155 30";
      minExtent = "26 27";
      visible = "1";
      hideCursor = "0";
      bypassHideCursor = "0";
      variable = "$pref::MyFlagStatus::EnableHeldTeamIcon";
      helpTag = "0";
      text = "Enable Flag On Stand Icon";
      longTextBuffer = "0";
      maxLength = "255";
    };
  };
} // setup function

}; // end package

// Activate this script
activatePackage(MyFlagStatus);

