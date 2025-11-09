// #autoload
// #name = oldKrets 2025 v1
// #config = kRETConfig

/// Updates by Foxox
/// Updated in 2025 for the Summer 2025 Tribes NEXT preview patches, which introduced a new FOV method and UI scaling.
/// Known issue: you have to cycle weapons to reset the krets display after changing your UI scaling setting.

$kPref::Mortar = 1;
$kPref::TankMortar = 1;
$kPref::Grenade = 1;
exec("prefs/KretPrefs.cs");

package KerbReticle {

/// foxox notes: Something to do with the main menu, like dedicated server, play online, etc. I think this is used as a place to put some initialization (adding the gui controls)
function DispatchLaunchMode()
{
	parent::DispatchLaunchMode();

  // foxox scratchwork. added these to see if i could get the script to be reloadable at runtime
  // playgui.remove(kMortarReticle);
	// playgui.remove(kGrenadeLauncherReticle);

	playgui.add(kMortarReticle);
	playgui.add(kGrenadeLauncherReticle);
}

//-----------------------------------------------------------------------------

/// Handles changes between player and vehicle HUD modes
function ClientCmdSetHudMode(%mode, %type, %node)
{
	parent::clientCmdSetHudMode(%mode, %type, %node);

	switch$($HudMode) {
	case "Standard":
		if($kReticle::blocked && $kReticle::mortarActive)
			kReticleShowMortar(1);
		if($kReticle::blocked && $kReticle::grenadeLauncherActive)
			kReticleShowGrenadeLauncher(1);
		$kReticle::blocked = 0;
	case "Pilot":
		if($kReticle::blocked && $kReticle::mortarActive)
			kReticleShowMortar(1);
		$kReticle::blocked = 0;
	default:
		kReticleShowMortar(0);
		kReticleShowGrenadeLauncher(0);
		$kReticle::blocked = 1;
	}
}

//-----------------------------------------------------------------------------

/// Handles weapon HUD changes
function clientCmdSetWeaponsHudActive(%slot)
{
	parent::clientCmdSetWeaponsHudActive(%slot);

	if(%slot == -1)
		kReticleDeactivate();
	else
		switch$($WeaponNames[%slot]) {
		case "GrenadeLauncher":
			kReticleActivateGrenadeLauncher();
		case "Mortar":
			kReticleActivateMortar(1);
		}
}

//-----------------------------------------------------------------------------

function clientCmdSetVWeaponsHudActive(%num, %vType)
{
	parent::clientCmdSetVWeaponsHudActive(%num, %vType);

	if(%vType $= "AssaultVehicle") {
		if(%num == 2)
			kReticleActivateMortar(2);
		else
			kReticleDeactivate();
	}
}

//-----------------------------------------------------------------------------

function clientCmdSetDefaultVehicleKeys(%inVehicle)
{
	parent::clientCmdSetDefaultVehicleKeys(%inVehicle);

	if(!%inVehicle && $kReticle::tankMortar)
		kReticleDeactivate();
}

//-----------------------------------------------------------------------------

function toggleZoom(%keydown)
{
	parent::toggleZoom(%keydown);

	kReticleOnChangeFOV();
}

//-----------------------------------------------------------------------------

function setFOV(%FOV)
{
	parent::setFOV(%FOV);
	
  // todo foxox try commenting this out?
	if($ZoomOn)
  kReticleOnChangeFOV();
}

function quit()
{
	parent::quit();
	export("$kPref::*", "prefs/KretPrefs.cs", false);
}

};

activatePackage(KerbReticle);

//-----------------------------------------------------------------------------

function kReticleOnChangeFOV()
{
	if($kReticle::mortarActive)
		kReticleActivateMortar();
	if($kReticle::grenadeLauncherActive)
		kReticleActivateGrenadeLauncher();
}

//-----------------------------------------------------------------------------

function kReticleShowMortar(%on)
{
	reticleHud.setVisible(!%on);
	reticleFrameHud.setVisible(!%on);
	kMortarReticle.setVisible(%on);
}

//-----------------------------------------------------------------------------

function buildBaseModTangentsArray()
{
  $kReticle::mortarTangent[0] = 0.1227;
  $kReticle::mortarTangent[1] = 0.1547;
  $kReticle::mortarTangent[2] = 0.1877;
  $kReticle::mortarTangent[3] = 0.2220;
  $kReticle::mortarTangent[4] = 0.2578;
  $kReticle::mortarTangent[5] = 0.2958;
  $kReticle::mortarTangent[6] = 0.3364;
  $kReticle::mortarTangent[7] = 0.3806;
  $kReticle::mortarTangent[8] = 0.4296;
  $kReticle::mortarTangent[9] = 0.4854;
  $kReticle::mortarTangent[10] = 0.5520;
  $kReticle::mortarTangent[11] = 0.6376;
  $kReticle::mortarTangent[12] = 0.7708;
  
  $kReticle::tankMortarTangent[0] = 0.1177;
  $kReticle::tankMortarTangent[1] = 0.1483;
  $kReticle::tankMortarTangent[2] = 0.1798;
  $kReticle::tankMortarTangent[3] = 0.2123;
  $kReticle::tankMortarTangent[4] = 0.2463;
  $kReticle::tankMortarTangent[5] = 0.2820;
  $kReticle::tankMortarTangent[6] = 0.3199;
  $kReticle::tankMortarTangent[7] = 0.3608;
  $kReticle::tankMortarTangent[8] = 0.4056;
  $kReticle::tankMortarTangent[9] = 0.4556;
  $kReticle::tankMortarTangent[10] = 0.5135;
  $kReticle::tankMortarTangent[11] = 0.5837;
  $kReticle::tankMortarTangent[12] = 0.6776;
  $kReticle::tankMortarTangent[13] = 0.8493;
  
  $kReticle::turretMortarTangent[0] = 0.1306;
  $kReticle::turretMortarTangent[1] = 0.2004;
  $kReticle::turretMortarTangent[2] = 0.2765;
  $kReticle::turretMortarTangent[3] = 0.3635;
  $kReticle::turretMortarTangent[4] = 0.4706;
  $kReticle::turretMortarTangent[5] = 0.6251;
  
  $kReticle::grenadeLauncherTangent[0] = 0.1124;
  $kReticle::grenadeLauncherTangent[1] = 0.1714;
  $kReticle::grenadeLauncherTangent[2] = 0.2342;
  $kReticle::grenadeLauncherTangent[3] = 0.3030;
  $kReticle::grenadeLauncherTangent[4] = 0.3816;
  $kReticle::grenadeLauncherTangent[5] = 0.4770;
  $kReticle::grenadeLauncherTangent[6] = 0.6086;
  $kReticle::grenadeLauncherTangent[7] = 0.9610;
}

// Classic Mod
function buildClassicModTangentsArray()
{
  $kReticle::mortarTangent[0] = 0.1338;
  $kReticle::mortarTangent[1] = 0.1670;
  $kReticle::mortarTangent[2] = 0.2002;
  $kReticle::mortarTangent[3] = 0.2334;
  $kReticle::mortarTangent[4] = 0.2705;
  $kReticle::mortarTangent[5] = 0.3096;
  $kReticle::mortarTangent[6] = 0.3545;
  $kReticle::mortarTangent[7] = 0.4033;
  $kReticle::mortarTangent[8] = 0.4561;
  $kReticle::mortarTangent[9] = 0.5186;
  $kReticle::mortarTangent[10] = 0.5947;
  $kReticle::mortarTangent[11] = 0.7021;
  $kReticle::mortarTangent[12] = 0.7508;
  
  $kReticle::tankMortarTangent[0] = 0.1318;
  $kReticle::tankMortarTangent[1] = 0.1572;
  $kReticle::tankMortarTangent[2] = 0.1865;
  $kReticle::tankMortarTangent[3] = 0.2139;
  $kReticle::tankMortarTangent[4] = 0.2451;
  $kReticle::tankMortarTangent[5] = 0.2783;
  $kReticle::tankMortarTangent[6] = 0.3115;
  $kReticle::tankMortarTangent[7] = 0.3486;
  $kReticle::tankMortarTangent[8] = 0.3896;
  $kReticle::tankMortarTangent[9] = 0.4346;
  $kReticle::tankMortarTangent[10] = 0.4834;
  $kReticle::tankMortarTangent[11] = 0.5420;
  $kReticle::tankMortarTangent[12] = 0.6103;
  $kReticle::tankMortarTangent[13] = 0.9999;  
  
  $kReticle::turretMortarTangent[0] = 0.0830;
  $kReticle::turretMortarTangent[1] = 0.1123;
  $kReticle::turretMortarTangent[2] = 0.1455;
  $kReticle::turretMortarTangent[3] = 0.1807;
  $kReticle::turretMortarTangent[4] = 0.2178;
  $kReticle::turretMortarTangent[5] = 0.2568;
  $kReticle::turretMortarTangent[6] = 0.2998;  
  
  $kReticle::grenadeLauncherTangent[0] = 0.1201;
  $kReticle::grenadeLauncherTangent[1] = 0.1768;
  $kReticle::grenadeLauncherTangent[2] = 0.2353;
  $kReticle::grenadeLauncherTangent[3] = 0.3037;
  $kReticle::grenadeLauncherTangent[4] = 0.3799;
  $kReticle::grenadeLauncherTangent[5] = 0.4736;
  $kReticle::grenadeLauncherTangent[6] = 0.5986;
  $kReticle::grenadeLauncherTangent[7] = 18.7801;
}

// V2 Mod - if Qing ever stops tweaking that thing I'll make the tangents ;)


/// Some helper functions added by foxox in order to deal with the 2025 Tribes Next patch

/// Gets the screen resolution, but scaled based on the UI Scale setting
/// @param[in] index Indicates whether you want horizontal or vertical. 0 = horizontal resolution, 1 = vertical resolution
function getUIScaledResolution(%index)
{
  return getword($pref::Video::resolution, %index) / getUIScale();
  //$pref::Video::uiScale;
}

/// Gets the horizontal screen center coordinate
function getHorizontalCenter()
{
  return mFloor(getword($pref::Video::resolution, 0) / 2);
}

/// Gets the vertical screen center coordinate
function getVerticalCenter()
{
  return mFloor(getword($pref::Video::resolution, 1) / 2);
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

/// This function adjusts screen scalar values (such as positions or extents) by the uiScale pref value
/// @param[in] scalar The scalar value to scale.
function uiScale(%scalar)
{
  return %scalar;
  
  // Attempt to get the krets UI not to scale with the rest of the UI. Not working yet, so disabling this...
  // return mFloor(%scalar / $pref::Video::uiScale);
}

/// Called when UI things change, like the scale
function onUpdateRenderTargets()
{
  parent::onUpdateRenderTargets();

  // Update reticle UI
  kReticleOnChangeFOV();   
}

/// End of foxox's helper functions


//-----------------------------------------------------------------------------

function kReticleActivateMortar(%mortarType)
{
	if(%mortarType == 1 && !$kPref::Mortar) return;
	else if(%mortarType == 2 && !$kPref::TankMortar) return;

	%paths = getModPaths();
	if(%paths $= "base") buildBaseModTangentsArray();
	else if(%paths $= "Classic;base") buildClassicModTangentsArray();
	else return;

	kMortarReticle.position = getUIScaledHorizontalCenter()-50 SPC getUIScaledVerticalCenter()-1;
	kMortarReticle.extent = "100 " @ getUIScaledVerticalCenter()+1;

	switch(%mortarType) {
	case 1:
		$kReticle::tankMortar = 0;
	case 2:
		$kReticle::tankMortar = 1;
	}
	
	%scaleFactor = 0.5 * getUIScaledResolution(1) /
	 mTan(($ZoomOn?$pref::player::currentFOV:$pref::player::defaultFOV) * 0.0087266);
	
	%currentY = mFloor(($kReticle::tankMortar?$kReticle::tankMortarTangent[0]:$kReticle::mortarTangent[0]) * %scaleFactor + 0.5);
	kMortarLine0.extent = 2 SPC %currentY-17;
	kMortarRange0.position = 34 SPC %currentY-5;

	for(%i = 1; %i < ($kReticle::tankMortar?14:13); %i++) {
		%lastY = %currentY;
		%currentY = mFloor(($kReticle::tankMortar?$kReticle::tankMortarTangent[%i]:$kReticle::mortarTangent[%i]) * %scaleFactor + 0.5);
		(kMortarLine @ %i).position = 49 SPC %lastY+3;
		(kMortarLine @ %i).extent = 2 SPC %currentY-%lastY-7;
		(kMortarRange @ %i).position = 34 SPC %currentY-5;
	}

	kMortarLine13.setVisible($kReticle::tankMortar);
	kMortarRange13.setVisible($kReticle::tankMortar);

	%lastY = %currentY;
	kMortarLine14.position = 49 SPC %lastY+3;

	$kReticle::mortarActive = 1;
	if(!$kReticle::blocked)
		kReticleShowMortar(1);
}

//-----------------------------------------------------------------------------

function kReticleShowGrenadeLauncher(%on)
{
	reticleHud.setVisible(!%on);
	reticleFrameHud.setVisible(!%on);
	kGrenadeLauncherReticle.setVisible(%on);
}

//-----------------------------------------------------------------------------

function kReticleActivateGrenadeLauncher()
{
	if(!$kPref::Grenade) return;

	%paths = getModPaths();
	if(%paths $= "base") buildBaseModTangentsArray();
	else if(%paths $= "Classic;base") buildClassicModTangentsArray();
	else return;

  // foxox scratchwork
  // echo("putting gren ret at vertical loc" SPC getUIScaledVerticalCenter());

	kGrenadeLauncherReticle.position = getUIScaledHorizontalCenter() - uiScale(50) SPC getUIScaledVerticalCenter() - 1;
	kGrenadeLauncherReticle.extent = uiScale(100) SPC getUIScaledVerticalCenter() + 1;

	%scaleFactor = 0.5 * getUIScaledResolution(1) /
	 mTan(($ZoomOn?$pref::player::currentFOV:$pref::player::defaultFOV) * 0.0087266);
	
	%currentY = mFloor($kReticle::grenadeLauncherTangent[0] * %scaleFactor + 0.5);
	kGrenadeLauncherLine0.extent = uiScale(2) SPC %currentY - uiScale(17);
	kGrenadeLauncherRange0.position = uiScale(34) SPC %currentY - uiScale(5);

	for(%i = 1; %i < 8; %i++) {
		%lastY = %currentY;
		%currentY = mFloor($kReticle::grenadeLauncherTangent[%i] * %scaleFactor + 0.5);
		(kGrenadeLauncherLine @ %i).position = uiScale(49) SPC %lastY + uiScale(3);
		(kGrenadeLauncherLine @ %i).extent = uiScale(2) SPC %currentY - %lastY - uiScale(7);
		(kGrenadeLauncherRange @ %i).position = uiScale(34) SPC %currentY - uiScale(5);
	}
	
	%lastY = %currentY;
	kGrenadeLauncherLine8.position = uiScale(49) SPC %lastY + uiScale(3);

	reticleHud.setVisible(0);
	reticleFrameHud.setVisible(0);
	$kReticle::grenadeLauncherActive = 1;
	if(!$kReticle::blocked)
		kReticleShowGrenadeLauncher(1);
}

//-----------------------------------------------------------------------------

function kReticleDeactivate()
{
	kReticleShowMortar(0);
	kReticleShowGrenadeLauncher(0);
	$kReticle::mortarActive = 0;
	$kReticle::grenadeLauncherActive = 0;
}

//-----------------------------------------------------------------------------

new ShellFieldCtrl(kMortarReticle) {
	profile = "GuiDefaultProfile";
	visible = "0";

	new GuiBitmapCtrl(kMortarTop) {
		profile = "GuiDefaultProfile";
		position = "33 0";
		visible = "1";
		extent = "34 13";
		opacity = "0.5";
		bitmap = "gui/kRET_mortarTop.png";
	};

	new GuiBitmapCtrl(kMortarLine0) {
		profile = "GuiDefaultProfile";
		position = "49 13";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange0) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		opacity = "0.5";
		bitmap = "gui/kRET_mortar100.png";
	};

	new GuiBitmapCtrl(kMortarLine1) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange1) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_mortar125.png";
	};

	new GuiBitmapCtrl(kMortarLine2) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange2) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_mortar150.png";
	};

	new GuiBitmapCtrl(kMortarLine3) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange3) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_mortar175.png";
	};

	new GuiBitmapCtrl(kMortarLine4) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange4) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_mortar200.png";
	};

	new GuiBitmapCtrl(kMortarLine5) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange5) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_mortar225.png";
	};

	new GuiBitmapCtrl(kMortarLine6) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange6) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_mortar250.png";
	};

	new GuiBitmapCtrl(kMortarLine7) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange7) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_mortar275.png";
	};

	new GuiBitmapCtrl(kMortarLine8) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange8) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_mortar300.png";
	};

	new GuiBitmapCtrl(kMortarLine9) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange9) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_mortar325.png";
	};

	new GuiBitmapCtrl(kMortarLine10) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange10) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_mortar350.png";
	};

	new GuiBitmapCtrl(kMortarLine11) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange11) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_mortar375.png";
	};

	new GuiBitmapCtrl(kMortarLine12) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange12) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_mortar400.png";
	};

	new GuiBitmapCtrl(kMortarLine13) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kMortarRange13) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_mortar425.png";
	};

	new GuiBitmapCtrl(kMortarLine14) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "2 1000";
		minExtent = "2 1000";
		bitmap = "gui/kRET_mortarLine.png";
		autoResize = "1";
	};
};

//-----------------------------------------------------------------------------

new ShellFieldCtrl(kGrenadeLauncherReticle) {
	profile = "GuiDefaultProfile";
	visible = "0";

	new GuiBitmapCtrl(kGrenadeLauncherTop) {
		profile = "GuiDefaultProfile";
		position = "33 0";
		visible = "1";
		extent = "34 13";
		opacity = "0.5";
		bitmap = "gui/kRET_grenadeTop.png";
	};

	new GuiBitmapCtrl(kGrenadeLauncherLine0) {
		profile = "GuiDefaultProfile";
		position = "49 13";
		visible = "1";
		bitmap = "gui/kRET_grenadeLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kGrenadeLauncherRange0) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_grenade50.png";
	};

	new GuiBitmapCtrl(kGrenadeLauncherLine1) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_grenadeLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kGrenadeLauncherRange1) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_grenade75.png";
	};

	new GuiBitmapCtrl(kGrenadeLauncherLine2) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_grenadeLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kGrenadeLauncherRange2) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		opacity = "0.5";
		bitmap = "gui/kRET_grenade100.png";
	};

	new GuiBitmapCtrl(kGrenadeLauncherLine3) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_grenadeLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kGrenadeLauncherRange3) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_grenade125.png";
	};

	new GuiBitmapCtrl(kGrenadeLauncherLine4) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_grenadeLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kGrenadeLauncherRange4) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_grenade150.png";
	};

	new GuiBitmapCtrl(kGrenadeLauncherLine5) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_grenadeLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kGrenadeLauncherRange5) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_grenade175.png";
	};

	new GuiBitmapCtrl(kGrenadeLauncherLine6) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_grenadeLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kGrenadeLauncherRange6) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_grenade200.png";
	};

	new GuiBitmapCtrl(kGrenadeLauncherLine7) {
		profile = "GuiDefaultProfile";
		visible = "1";
		bitmap = "gui/kRET_grenadeLine.png";
		autoResize = "1";
	};

	new GuiBitmapCtrl(kGrenadeLauncherRange7) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "62 9";
		bitmap = "gui/kRET_grenade225.png";
	};

	new GuiBitmapCtrl(kGrenadeLauncherLine8) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "2 1000";
		minExtent = "2 1000";
		bitmap = "gui/kRET_grenadeLine.png";
		autoResize = "1";
	};
};

//-----------------------------------------------------------------------------

new GuiControl(kRETConfig) {
	profile = "GuiDefaultProfile";
	horizSizing = "center";
	vertSizing = "center";
	position = "-120 -64";
	extent = "640 480";
	minExtent = "8 8";
	visible = "1";
	hideCursor = "0";
	bypassHideCursor = "0";
	helpTag = "0";
	new ShellPaneCtrl() {
		profile = "ShellPaneProfile";
		horizSizing = "center";
		vertSizing = "center";
		position = "194 165";
		extent = "251 150";
		minExtent = "48 92";
		visible = "1";
		hideCursor = "0";
		bypassHideCursor = "0";
		helpTag = "0";
		text = "kRet";
		longTextBuffer = "0";
		maxLength = "255";
		noTitleBar = "0";
		new ShellFieldCtrl() {
			profile = "ShellFieldProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "20 29";
			extent = "211 100";
			minExtent = "16 18";
			visible = "1";
			hideCursor = "0";
			bypassHideCursor = "0";
			helpTag = "0";
			new ShellToggleButton(kRetGrenLauncherButton) {
				profile = "ShellRadioProfile";
				horizSizing = "center";
				vertSizing = "bottom";
				position = "0 5";
				extent = "155 30";
				minExtent = "26 27";
				visible = "1";
				hideCursor = "0";
				bypassHideCursor = "0";
				helpTag = "0";
				text = "GrenadeLauncher";
				variable = "$kPref::Grenade";
				longTextBuffer = "0";
				maxLength = "255";
			};
			new ShellToggleButton(kRETMortarButton) {
				profile = "ShellRadioProfile";
				horizSizing = "center";
				vertSizing = "bottom";
				position = "0 35";
				extent = "155 30";
				minExtent = "26 27";
				visible = "1";
				hideCursor = "0";
				bypassHideCursor = "0";
				helpTag = "0";
				text = "Mortar";
				variable = "$kPref::Mortar";
				longTextBuffer = "0";
				maxLength = "255";
			};
			new ShellToggleButton(kRETTankMortarButton) {
				profile = "ShellRadioProfile";
				horizSizing = "center";
				vertSizing = "bottom";
				position = "0 65";
				extent = "155 30";
				minExtent = "26 27";
				visible = "1";
				hideCursor = "0";
				bypassHideCursor = "0";
				helpTag = "0";
				text = "Tank Mortar";
				variable = "$kPref::TankMortar";
				longTextBuffer = "0";
				maxLength = "255";
			};
		};
	};
};
