// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;



[CustomEditor(typeof(PrefabShooter))]
public class PrefabShooterEditor : Editor
{

	public override void OnInspectorGUI()
	{
		PrefabShooter shooter = target as PrefabShooter;

		m_obstacleLayers.Enabled = shooter.m_checkForObstacles;
		
		m_editHelp.EditProperties( shooter );

		
	}
	
	public void OnEnable()
	{
		m_editHelp = new EditHelp();
		
		m_editHelp.AddPropertyInterface( 
			"m_bulletVelocity", new EditHelp.PropertyInterface(
				"Bullet speed", 
				"The speed that the bullet leaves the barrel of the gun at.") );
		
		m_editHelp.AddPropertyInterface( 
			"m_bulletPrefab", new EditHelp.PropertyInterface(
				"Bullet prefab",
				"Prefab to get instanced when creating a bullet.") );
		
		m_editHelp.AddPropertyInterface( 
			"m_firingPause", new EditHelp.PropertyInterface(
				"Firing Pause",
				"The amount of time (in seconds) to pause between shots.") );
		
		m_editHelp.AddPropertyInterface( 
			"m_bulletLifetime", new EditHelp.PropertyInterface(
				"Bullet Lifetime",
				"The amount of time (in seconds) the bullet will exist before being destroyed.") );
		
		m_editHelp.AddPropertyInterface( 
			"m_firingCone", new EditHelp.SliderPropertyInterface(
				"Firing cone",
				"This is the angle of the cone in which the bullets will be fired, in degrees.\n" +
				"Good to use if you don't want your turrets to be too accurate.\n" +
				"0 = deadly accurate, 30 = can barely point in the right direction.",
				0, 90.0f) );
		
		m_editHelp.AddPropertyInterface( 
			"m_firingAnimation", new EditHelp.PropertyInterface(
				"Firing Animation",
				"An animation to play when the gun fires.") );
		
		m_editHelp.AddPropertyInterface( 
			"m_firingMessage", new EditHelp.PropertyInterface(
				"Firing Message",
				"An message that gets sent when the gun fires.") );
		
		m_editHelp.AddPropertyInterface( 
			"m_exitPoints", new EditHelp.ArrayPropertyInterface(
				"Projectile Exit Points",
				"Exit points\nYour turrent can have several projectile exit points - one per barrel. Set them up here.\n" +
				"Attributes are:\n" + 
				" * Firing Animation : An animation that will get triggered when shooting.\n" +
				" * Firing Offset : The offset that the bullet gets shot from.\n",
				"Exit Point") );
		
		m_editHelp.AddPropertyInterface( 
			"m_checkForObstacles", new EditHelp.BoolPropertyInterface(
				"Check for obstacles",
				"Whether or not to stop shooting when an obstacle is between the turret and its target.\n" +
				"NOTE: inaccurate for highly parabolic shots.",
				"TRUE: Gun will cease firing when an obstacle is in the way.",
				"FALSE: Gun will shoot irrespective of whether an obstacle is in the way.") );

		m_obstacleLayers = new EditHelp.PropertyInterface (
			"Obstacle Check Layers",
			"Which layers to include when checking for obstacles in front of the target");

		m_editHelp.AddPropertyInterface ("m_obstacleLayers", m_obstacleLayers);
			
	}
	
	public void OnSceneGUI()
	{
		// make sure we detect any changes in the object
		GUI.changed = false;
		
		PrefabShooter shooter = target as PrefabShooter;		
		Aimer aimer = shooter.GetComponent<Aimer>();
		
		if (!aimer || !aimer.m_gunObject)
		{
			return;
		}
		
		Matrix4x4 handlesMatrix = Handles.matrix;
		Handles.matrix = aimer.m_gunObject.transform.localToWorldMatrix;
		Color oldColor = Handles.color;
		Handles.color = Color.red;
		
		foreach (PrefabShooter.ExitPoint point in shooter.m_exitPoints)
		{
			Handles.DrawWireDisc(point.firingOffset, Vector3.forward, 0.2f);
			
			point.firingOffset = Handles.PositionHandle(point.firingOffset, Quaternion.identity);
		}
		
		Handles.color = oldColor;
		Handles.matrix = handlesMatrix;
		
		// report any changes in the object.
		if (GUI.changed)
		{
			EditorUtility.SetDirty(shooter);
		}
	}

	
	private EditHelp m_editHelp;
	private EditHelp.PropertyInterface m_obstacleLayers;
}

