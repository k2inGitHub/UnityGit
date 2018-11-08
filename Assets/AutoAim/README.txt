     _   _   _ _____ ___    _    ___ __  __ 
    / \ | | | |_   _/ _ \  / \  |_ _|  \/  |
   / _ \| | | | | || | | |/ _ \  | || |\/| |
  / ___ \ |_| | | || |_| / ___ \ | || |  | |
 /_/   \_\___/  |_| \___/_/   \_\___|_|  |_|
  
                    By 
  
     ______
    /_____ \________________________
      ^   \                         \
    < o > |   The Game Mechanics  o |
    __v___/ ________________________/
    \______/
 
      Copyright © 2012 - 2015 Timothy Aidley
 
 
DOCUMENTATION:
==============

 The main documentation for AutoAim can be found on The Game Mechanics wesbite,
 at the following address:
 
 	http://www.thegamemechanics.co.uk/autoaim/
 
SUPPORT:
========

 Support is available through The Game Machanics website forum, at:
 
    http://www.thegamemechanics.co.uk/forum/   
 
 
LICENSE:
========
 
 This software is licensed under the Unity Asset Store Terms of Service EULA:
 
 http://unity3d.com/unity/asset-store/docs/customer-eula.html
 
 
WARRANTY:
=========

 The Software is provided "as is" without warranty of any kind.
 
 
FILES:
======

All Autoaim code resides under the 'AutoAim' directory. You can change this directory
but doing so may well break the images used in the Turret Wizard. All other functionality
should remain unchanged.

AutoAim
  |
  +-- Scripts : Contains the AutoAim components.
  |     |
  |     +-- Aiming : Contains aiming libraries used by the AutoAim components.
  |
  +-- Editor : Contains the scripts that provide improved Inspector and Scene editing.
  |     |      This can be removed, but you will lose the improved editing functionality.
  |     |
  |     +-- Resources : Contains images used by the Turret Wizard.
  |
  +-- Demo : Holds all the demo files; you may safely remove this directory
       |     and all its contents for your own projects.
       |
       +-- Animation : Animation used in the Linear demo scene.
       |
       +-- Materials : Materials used in the demo scenes.
       |
       +-- Prefabs   : Prefabs used in the demo scenes.
       |
       +-- Props     : Terrain used in the demo scenes.
       |
       +-- Scenes    : The demo scenes.
       |
       +-- Scripts   : Some simple helper scripts used to build the demo scenes.
	   
	   
CHANGELOG:
==========

Version 1.07:
-------------
* Make sure gun barrel is always 'up' when compared to the base of the turret.
* Unity 5.0 support.

Version 1.06:
-------------
* Fix for bug where first shot could be off when using linear / linear aiming.
* Added ability to filter layers tested against when checking for obstacles.
* Fixed warnings that resulted from methods that were deprecated in Unity version 4.0.

Version 1.05:
-------------
* Fix for issue where angular-velocity limited turret could get stuck pointing directly upwards
  if the target flies directly overhead (relative to the turret base).

Version 1.04:
-------------
* Now supports moving the install directory to another location. Moving it previously would break
  the turret wizard.

Version 1.03:
-------------
* Adds a 'Check for obstacles' option to the prefab shooter. When checked, this will not shoot
  when an obstacle falls between the gun and the target.

Version 1.02:
-------------
 * Adds Projectile exit points so that you can (optionally) choose a set of points to shoot
   from. This is designed to make multi-barrelled guns easy.
 * Adds an option on PrefabShooter to Send a message on every shot.
 * Adds new demo scenes demonstrating multiple barrels and message sending.

Version 1.01:
-------------
 * Fixed a bug in the aiming code for dual-section turrets that prevented non-horizontal
   turrets from rotating their swivel base properly.

Version 1.00:
-------------
 * Initial revision.