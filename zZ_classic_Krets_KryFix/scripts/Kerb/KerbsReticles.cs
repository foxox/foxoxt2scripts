// #autoload
// #name = Krets
// #config = kRETConfig
// #include = support/vehicle_callbacks.cs 0.0.5

$kPref::Mortar = 1;
$kPref::TankMortar = 1;
$kPref::TurretMortar = 1;
$kPref::Grenade = 1;
exec("prefs/KretPrefs.cs");

// register callbacks
callback.add(onVehicleDismount, kScriptDeactivateReticles);

//*****************************************************************************
// OVERRIDES
//*****************************************************************************

package kScriptOverrides {

//-----------------------------------------------------------------------------

function OptionsDlg::onWake( %this ) {
	if($kScriptRangefinderRemap != 1) {
		$RemapName[$RemapCount]="Rangefinder";
		$RemapCmd[$RemapCount]="kScriptRangefinder";
		$RemapCount++;
		$kScriptRangefinderRemap = 1;
	}
	parent::onWake( %this );
}

//-----------------------------------------------------------------------------

function PlayGui::onWake(%this) {
	parent::onWake(%this);
	PlayGui.add(kScriptMortarReticle);
	PlayGui.add(kScriptGrenadeReticle);
}

//-----------------------------------------------------------------------------
function setFOV(%fov)
{
	parent::setFOV(%fov);
	if($kScript::mortarReticleActive)
		kScriptBuildMortarReticle(0);
	if($kScript::grenadeReticleActive)
		kScriptBuildGrenadeReticle();
}

//-----------------------------------------------------------------------------
function quit()
{
	parent::quit();
	export("$kPref::*", "prefs/KretPrefs.cs", false);
}

//-----------------------------------------------------------------------------
function ClientCmdSetHudMode(%mode, %type, %node)
{
	parent::clientCmdSetHudMode(%mode, %type, %node);

	switch$($HudMode) {
	case "Standard":
		kScriptBlockReticles(0);
		kScriptRefreshInfantryReticles();
	case "Pilot":
		kScriptBlockReticles(0);
	case "Object":
		// controlling an object always unzooms you
		kScriptDeactivateReticles();
		if($HudModeType $= "MortarBarrelLarge") {
			kScriptBlockReticles(0);
			kScriptBuildMortarReticle(3);
		}
	default:
		kScriptBlockReticles(1);
	}
	kScriptFixFOV();
}

//-----------------------------------------------------------------------------
function ammoHud::setVisible(%this, %on)
{
	parent::setVisible(%this, %on);
	kScriptBlockReticles(!%on);
}

//-----------------------------------------------------------------------------

// [PEN]Koko - Do not display ranged reticles for Team Rabbit 2
function clientCmdSetWeaponsHudActive(%slot, %ret, %vis)
{
	parent::clientCmdSetWeaponsHudActive(%slot, %ret, %vis);

	if(%slot == -1)
		kScriptDeactivateReticles();
	else
		switch$($WeaponNames[%slot]) {
		case "GrenadeLauncher":
			kScriptBuildGrenadeReticle();
//		case "TR2GrenadeLauncher":
//			kScriptBuildGrenadeReticle();
		case "Mortar":
			kScriptBuildMortarReticle(1);
//		case "TR2Mortar":
//			kScriptBuildMortarReticle(1);
	}
}

//-----------------------------------------------------------------------------

function clientCmdSetVWeaponsHudActive(%num, %vType)
{
	parent::clientCmdSetVWeaponsHudActive(%num, %vType);

	if(%vType $= "AssaultVehicle") {
		if(%num == 2)
			kScriptBuildMortarReticle(2);
		else
			kScriptDeactivateReticles();
	}
}
};
activatePackage(kScriptOverrides);

//*****************************************************************************
// MORTAR/GRENADE RETICLES
//*****************************************************************************

// Base Mod
function buildBaseModTangentsArray()
{
  $kScript::mortarTangent[0] = 0.1227;
  $kScript::mortarTangent[1] = 0.1547;
  $kScript::mortarTangent[2] = 0.1877;
  $kScript::mortarTangent[3] = 0.2220;
  $kScript::mortarTangent[4] = 0.2578;
  $kScript::mortarTangent[5] = 0.2958;
  $kScript::mortarTangent[6] = 0.3364;
  $kScript::mortarTangent[7] = 0.3806;
  $kScript::mortarTangent[8] = 0.4296;
  $kScript::mortarTangent[9] = 0.4854;
  $kScript::mortarTangent[10] = 0.5520;
  $kScript::mortarTangent[11] = 0.6376;
  $kScript::mortarTangent[12] = 0.7708;
  
  $kScript::tankMortarTangent[0] = 0.1177;
  $kScript::tankMortarTangent[1] = 0.1483;
  $kScript::tankMortarTangent[2] = 0.1798;
  $kScript::tankMortarTangent[3] = 0.2123;
  $kScript::tankMortarTangent[4] = 0.2463;
  $kScript::tankMortarTangent[5] = 0.2820;
  $kScript::tankMortarTangent[6] = 0.3199;
  $kScript::tankMortarTangent[7] = 0.3608;
  $kScript::tankMortarTangent[8] = 0.4056;
  $kScript::tankMortarTangent[9] = 0.4556;
  $kScript::tankMortarTangent[10] = 0.5135;
  $kScript::tankMortarTangent[11] = 0.5837;
  $kScript::tankMortarTangent[12] = 0.6776;
  $kScript::tankMortarTangent[13] = 0.8493;
  
  $kScript::turretMortarTangent[0] = 0.1306;
  $kScript::turretMortarTangent[1] = 0.2004;
  $kScript::turretMortarTangent[2] = 0.2765;
  $kScript::turretMortarTangent[3] = 0.3635;
  $kScript::turretMortarTangent[4] = 0.4706;
  $kScript::turretMortarTangent[5] = 0.6251;
  
  $kScript::grenadeTangent[0] = 0.1124;
  $kScript::grenadeTangent[1] = 0.1714;
  $kScript::grenadeTangent[2] = 0.2342;
  $kScript::grenadeTangent[3] = 0.3030;
  $kScript::grenadeTangent[4] = 0.3816;
  $kScript::grenadeTangent[5] = 0.4770;
  $kScript::grenadeTangent[6] = 0.6086;
  $kScript::grenadeTangent[7] = 0.9610;
}

// Classic Mod
function buildClassicModTangentsArray()
{
  $kScript::mortarTangent[0] = 0.1338;
  $kScript::mortarTangent[1] = 0.1670;
  $kScript::mortarTangent[2] = 0.2002;
  $kScript::mortarTangent[3] = 0.2334;
  $kScript::mortarTangent[4] = 0.2705;
  $kScript::mortarTangent[5] = 0.3096;
  $kScript::mortarTangent[6] = 0.3545;
  $kScript::mortarTangent[7] = 0.4033;
  $kScript::mortarTangent[8] = 0.4561;
  $kScript::mortarTangent[9] = 0.5186;
  $kScript::mortarTangent[10] = 0.5947;
  $kScript::mortarTangent[11] = 0.7021;
  $kScript::mortarTangent[12] = 0.7508;
  
  $kScript::tankMortarTangent[0] = 0.1318;
  $kScript::tankMortarTangent[1] = 0.1572;
  $kScript::tankMortarTangent[2] = 0.1865;
  $kScript::tankMortarTangent[3] = 0.2139;
  $kScript::tankMortarTangent[4] = 0.2451;
  $kScript::tankMortarTangent[5] = 0.2783;
  $kScript::tankMortarTangent[6] = 0.3115;
  $kScript::tankMortarTangent[7] = 0.3486;
  $kScript::tankMortarTangent[8] = 0.3896;
  $kScript::tankMortarTangent[9] = 0.4346;
  $kScript::tankMortarTangent[10] = 0.4834;
  $kScript::tankMortarTangent[11] = 0.5420;
  $kScript::tankMortarTangent[12] = 0.6103;
  $kScript::tankMortarTangent[13] = 0.9999;  
  
  $kScript::turretMortarTangent[0] = 0.0830;
  $kScript::turretMortarTangent[1] = 0.1123;
  $kScript::turretMortarTangent[2] = 0.1455;
  $kScript::turretMortarTangent[3] = 0.1807;
  $kScript::turretMortarTangent[4] = 0.2178;
  $kScript::turretMortarTangent[5] = 0.2568;
  $kScript::turretMortarTangent[6] = 0.2998;  
  
  $kScript::grenadeTangent[0] = 0.1201;
  $kScript::grenadeTangent[1] = 0.1768;
  $kScript::grenadeTangent[2] = 0.2353;
  $kScript::grenadeTangent[3] = 0.3037;
  $kScript::grenadeTangent[4] = 0.3799;
  $kScript::grenadeTangent[5] = 0.4736;
  $kScript::grenadeTangent[6] = 0.5986;
  $kScript::grenadeTangent[7] = 18.7801;
}

// V2 Mod - if Qing ever stops tweaking that thing I'll make the tangents ;)

//-----------------------------------------------------------------------------

function kScriptBuildMortarReticle(%mortarType)
{
	if(%mortarType == 1 && !$kPref::Mortar) return;
	else if(%mortarType == 2 && !$kPref::TankMortar) return;
	else if(%mortarType == 3 && !$kPref::TurretMortar) return;

// [PEN]Koko - if anyone knows a better way to test for mod type fix this.
// A server operator can change the name of the mod directory, causing this
// test to fail.

// %paths = "Classic;base" for Classic mod
// %paths = "base" for base

  %paths = getModPaths();
  if(%paths $= "base")
  {
    buildBaseModTangentsArray();
  } 
  else if(%paths $= "Classic;base")
  {
    buildClassicModTangentsArray();
  }
  else
  {
    return;
  }

  
	kScriptMortarReticle.position = getword($pref::Video::resolution, 0)/2-16 @ " " @ getword($pref::Video::resolution, 1)/2-1;
	kScriptMortarReticle.extent = "63 " @ getword($pref::Video::resolution, 1)/2+1;

	%scaleFactor = 0.5 * getword($pref::Video::resolution, 0) /
	 mTan(($ZoomOn?$pref::player::currentFOV:$pref::player::defaultFOV) * 0.0087266);
	
	if(%mortarType == 0)
		%mortarType = $kScript::lastMortarType;
	$kScript::lastMortarType = %mortarType;
	
	switch(%mortarType) {
	case 1:
		// infantry mortar
		%tickOffset = 2;
		%numTicks = 12;
	case 2:
		// tank mortar
		%tickOffset = 2;
		%numTicks = 13;
	case 3:
		// turret mortar
		%tickOffset = 0;
		%numTicks = 6;
	}

	for(%i = 0; %i <= %numTicks; %i++) {
		switch(%mortarType) {
		case 1:
			%currentTangent = $kScript::mortarTangent[%i];
		case 2:
			%currentTangent = $kScript::tankMortarTangent[%i];
		case 3:
			%currentTangent = $kScript::turretMortarTangent[%i];
		}
		%currentY = mFloor(%currentTangent * %scaleFactor + 0.5);
		(KSR_mortarTick @ (%i + %tickOffset)).position = "0 " @ %currentY-4;
		if(!(%i & 1) || %i == 13)
			(KSR_mortarRange @ (%i + %tickOffset)).position = "32 " @ %currentY-5;
	}

	// show ranges 50 and 75 only for turret mortar
	KSR_mortarTick0.setVisible(%mortarType == 3);
	KSR_mortarRange0.setVisible(%mortarType == 3);
	KSR_mortarTick1.setVisible(%mortarType == 3);

	
	// don't show 200-400 for turret mortar
	KSR_mortarTick6.setVisible(%mortarType != 3);
	KSR_mortarRange6.setVisible(%mortarType != 3);
	KSR_mortarTick7.setVisible(%mortarType != 3);
	KSR_mortarTick8.setVisible(%mortarType != 3);
	KSR_mortarRange8.setVisible(%mortarType != 3);
	KSR_mortarTick9.setVisible(%mortarType != 3);
	KSR_mortarTick10.setVisible(%mortarType != 3);
	KSR_mortarRange10.setVisible(%mortarType != 3);
	KSR_mortarTick11.setVisible(%mortarType != 3);
	KSR_mortarTick12.setVisible(%mortarType != 3);
	KSR_mortarRange12.setVisible(%mortarType != 3);
	KSR_mortarTick13.setVisible(%mortarType != 3);

// Removed by [PEN]Koko - 400 meter ticks
//	KSR_mortarTick14.setVisible(%mortarType != 3);
//	KSR_mortarRange14.setVisible(%mortarType != 3);
	
	// show 400 and 425 only for tank mortar
	KSR_mortarTick14.setVisible(%mortarType == 2);
	KSR_mortarRange14.setVisible(%mortarType == 2);
	KSR_mortarTick15.setVisible(%mortarType == 2);
	KSR_mortarRange15.setVisible(%mortarType == 2);

	$kScript::mortarReticleActive = 1;
	if(!$kScript::blockReticles)
		kScriptMortarReticle.setVisible(1);
}

//-----------------------------------------------------------------------------

function kScriptBuildGrenadeReticle()
{
	if(!$kPref::Grenade) return;

  %paths = getModPaths();
  if(%paths $= "base")
  {
    buildBaseModTangentsArray();
  } 
  else if(%paths $= "Classic;base")
  {
    buildClassicModTangentsArray();
  }
  else
  {
    return;
  }

	kScriptGrenadeReticle.position = getword($pref::Video::resolution, 0)/2-16 @ " " @ getword($pref::Video::resolution, 1)/2-1;
	kScriptGrenadeReticle.extent = "63 " @ getword($pref::Video::resolution, 1)/2+1;

	%scaleFactor = 0.5 * getword($pref::Video::resolution, 0) /
	 mTan(($ZoomOn?$pref::player::currentFOV:$pref::player::defaultFOV) * 0.0087266);
	
	for(%i = 0; %i <= 7; %i++) {
		%currentY = mFloor($kScript::grenadeTangent[%i] * %scaleFactor + 0.5);
		(KSR_grenadeTick @ %i).position = "0 " @ %currentY-4;
		if(!(%i & 1) || %i == 7)
			(KSR_grenadeRange @ %i).position = "32 " @ %currentY-5;
	}

	$kScript::grenadeReticleActive = 1;
	if(!$kScript::blockReticles)
		kScriptGrenadeReticle.setVisible(1);
}

//-----------------------------------------------------------------------------

function kScriptDeactivateReticles()
{
	kScriptMortarReticle.setVisible(0);
	kScriptGrenadeReticle.setVisible(0);
	$kScript::mortarReticleActive = 0;
	$kScript::grenadeReticleActive = 0;
}

//-----------------------------------------------------------------------------

function kScriptBlockReticles(%block)
{
	if(%block) {
		kScriptMortarReticle.setVisible(0);
		kScriptGrenadeReticle.setVisible(0);
		$kScript::blockReticles = 1;
	}
	else {
		if($kScript::blockReticles && $kScript::mortarReticleActive)
			kScriptMortarReticle.setVisible(1);
		if($kScript::blockReticles && $kScript::grenadeReticleActive)
			kScriptGrenadeReticle.setVisible(1);
		$kScript::blockReticles = 0;
	}
}

//-----------------------------------------------------------------------------

function kScriptRefreshInfantryReticles()
{
	kScriptDeactivateReticles();

	switch$(loadout.getCurrentWeapon()) {
	case "GrenadeLauncher":
		kScriptBuildGrenadeReticle();
	case "Mortar":
		kScriptBuildMortarReticle(1);
	}
}

//*****************************************************************************
// MISC
//*****************************************************************************

//-----------------------------------------------------------------------------
function kScriptFixFOV()
{
	// make sure fov is correct if we're unzoomed... T2 has a nasty habit
	// of resetting fov to 90 instead of $pref::player::defaultFOV.
	schedule(200, 0, kScriptCheckUnzoom);
	schedule(600, 0, kScriptCheckUnzoom); // safety
}

//-----------------------------------------------------------------------------

function kScriptCheckUnzoom()
{
	if(!$ZoomOn)
		setFOV($pref::player::defaultFOV);
}


//-----------------------------------------------------------------------------

function kScriptRangefinder(%keydown)
{
	if(%keydown) {
		useTargetingLaser(1);
		if(!($mvTriggerCount0 & 1))
			$mvTriggerCount0++;
		schedule(1000, 0, kScriptStopRangefinder);
	}
}

//-----------------------------------------------------------------------------

function kScriptStopRangefinder()
{
	if($mvTriggerCount0 & 1)
		$mvTriggerCount0++;
	
	loadout.useWeapon(loadout.getPreviousWeapon());
}


//-----------------------------------------------------------------------------

// Reticle Gui Objects for Kerb's T2 Scripts
//
// (c) 2002 David Gausebeck
// gausebec@paypal.com

//-----------------------------------------------------------------------------

new ShellFieldCtrl(kScriptMortarReticle) {
	profile = "GuiDefaultProfile";
	visible = "0";

	// 50
	new GuiBitmapCtrl(KSR_mortarTick0) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};
	new GuiBitmapCtrl(KSR_mortarRange0) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "31 11";
		bitmap = "Kerb/kRET_mortar50.png";
	};

	// 75
	new GuiBitmapCtrl(KSR_mortarTick1) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};

	// 100
	new GuiBitmapCtrl(KSR_mortarTick2) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};
	new GuiBitmapCtrl(KSR_mortarRange2) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "31 11";
		bitmap = "Kerb/kRET_mortar100.png";
	};

	// 125
	new GuiBitmapCtrl(KSR_mortarTick3) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};

	// 150
	new GuiBitmapCtrl(KSR_mortarTick4) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};
	new GuiBitmapCtrl(KSR_mortarRange4) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "31 11";
		bitmap = "Kerb/kRET_mortar150.png";
	};

	// 175
	new GuiBitmapCtrl(KSR_mortarTick5) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};

	// 200
	new GuiBitmapCtrl(KSR_mortarTick6) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};
	new GuiBitmapCtrl(KSR_mortarRange6) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "31 11";
		bitmap = "Kerb/kRET_mortar200.png";
	};

	// 225
	new GuiBitmapCtrl(KSR_mortarTick7) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};

	// 250
	new GuiBitmapCtrl(KSR_mortarTick8) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};
	new GuiBitmapCtrl(KSR_mortarRange8) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "31 11";
		bitmap = "Kerb/kRET_mortar250.png";
	};

	// 275
	new GuiBitmapCtrl(KSR_mortarTick9) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};

	// 300
	new GuiBitmapCtrl(KSR_mortarTick10) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};
	new GuiBitmapCtrl(KSR_mortarRange10) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "31 11";
		bitmap = "Kerb/kRET_mortar300.png";
	};

	// 325
	new GuiBitmapCtrl(KSR_mortarTick11) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};

	// 350
	new GuiBitmapCtrl(KSR_mortarTick12) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};
	new GuiBitmapCtrl(KSR_mortarRange12) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "31 11";
		bitmap = "Kerb/kRET_mortar350.png";
	};

	// 375
	new GuiBitmapCtrl(KSR_mortarTick13) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};

	// 400
	new GuiBitmapCtrl(KSR_mortarTick14) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};
	new GuiBitmapCtrl(KSR_mortarRange14) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "31 11";
		bitmap = "Kerb/kRET_mortar400.png";
	};

	// 425
	new GuiBitmapCtrl(KSR_mortarTick15) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_mortarTick.png";
	};
	new GuiBitmapCtrl(KSR_mortarRange15) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "31 11";
		bitmap = "Kerb/kRET_mortar425.png";
	};
};

//-----------------------------------------------------------------------------

new ShellFieldCtrl(kScriptGrenadeReticle) {
	profile = "GuiDefaultProfile";
	visible = "0";

	// 50
	new GuiBitmapCtrl(KSR_grenadeTick0) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_grenadeTick.png";
	};
	new GuiBitmapCtrl(KSR_grenadeRange0) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "31 11";
		bitmap = "Kerb/kRET_grenade50.png";
	};

	// 75
	new GuiBitmapCtrl(KSR_grenadeTick1) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_grenadeTick.png";
	};

	// 100
	new GuiBitmapCtrl(KSR_grenadeTick2) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_grenadeTick.png";
	};
	new GuiBitmapCtrl(KSR_grenadeRange2) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "31 11";
		bitmap = "Kerb/kRET_grenade100.png";
	};

	// 125
	new GuiBitmapCtrl(KSR_grenadeTick3) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_grenadeTick.png";
	};

	// 150
	new GuiBitmapCtrl(KSR_grenadeTick4) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_grenadeTick.png";
	};
	new GuiBitmapCtrl(KSR_grenadeRange4) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "31 11";
		bitmap = "Kerb/kRET_grenade150.png";
	};

	// 175
	new GuiBitmapCtrl(KSR_grenadeTick5) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_grenadeTick.png";
	};

	// 200
	new GuiBitmapCtrl(KSR_grenadeTick6) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_grenadeTick.png";
	};
	new GuiBitmapCtrl(KSR_grenadeRange6) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "31 11";
		bitmap = "Kerb/kRET_grenade200.png";
	};

	// 225
	new GuiBitmapCtrl(KSR_grenadeTick7) {
		profile = "GuiDefaultProfile";
		visible = "1";
		extent = "32 9";
		bitmap = "Kerb/kRET_grenadeTick.png";
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
		extent = "251 180";
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
			extent = "211 130";
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
			new ShellToggleButton(kRETTurretMortarButton) {
				profile = "ShellRadioProfile";
				horizSizing = "center";
				vertSizing = "bottom";
				position = "0 95";
				extent = "155 30";
				minExtent = "26 27";
				visible = "1";
				hideCursor = "0";
				bypassHideCursor = "0";
				helpTag = "0";
				text = "Mortar Turret";
				variable = "$kPref::TurretMortar";
				longTextBuffer = "0";
				maxLength = "255";
			};
		};
	};
};
