// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


[CustomEditor(typeof(Aimer))]
public class AimerEditor : Editor
{
	
	void OnEnable()
	{
		m_editHelp.AddPropertyInterface(
			"m_targetObject", new EditHelp.PropertyInterface(
				"Target to aim at",
				"This is the object that the aimer will be aiming at.") );
		
		m_editHelp.AddPropertyInterface(
			"m_targetType", new EditHelp.EnumPropertyInterface(
				"Target Type",
				"The type of movement the target is expected to have.",
				"Linear: targets move at a constant speed in a straight line unaffacted by gravity.",
				"Parabolic: targets move in an arc, affected by gravity.",
				"Circular: Use for players and AI-controlled targets.\nTarget motion is estimated using a curve.") );
				//"Autodetect: detect target type automatically (uses more CPU)") );
		
		m_editHelp.AddPropertyInterface(
			"m_projectileType", new EditHelp.EnumPropertyInterface(
				"Projectile Type",
				"Choose Projectile type:",
				"Instant: Projectile hits the target instantaneously",
				"Linear: Projectile moves at a constant speed in a straight line.",
				"Parabolic: Projectile moves affected only by gravity." ) );
		
		m_editHelp.AddPropertyInterface(
			"m_bulletVelocity", new EditHelp.PropertyInterface(
				"Projectile speed",
				"Speed the projectile is launched at.") );
		
		m_gunObject = new EditHelp.PropertyInterface(
				"Gun barrel object",
				"This is the object that will be aimed to point in the firing direction.");
		m_editHelp.AddPropertyInterface( "m_gunObject", m_gunObject );
		
		m_swivelObject = new EditHelp.PropertyInterface(
				"Optional Swivel Base object",
				"This property is optional. If set, it is used at the swivel base for the turret.");
		m_editHelp.AddPropertyInterface( "m_optionalSwivel", m_swivelObject );
		
		m_bulletSpeed = new EditHelp.PropertyInterface(
				"Bullet speed",
				"This should match the speed that your bullets are launched at.");
		m_editHelp.AddPropertyInterface( "m_projectileSpeed", m_bulletSpeed );
		
		m_editHelp.AddPropertyInterface(
			"m_artificalStupidity", new EditHelp.SliderPropertyInterface(
				"Artificial Stupidity",
				"Use this to make the aimer a little less mercilessly accurate.\n" +
				"If set to zero, the aiming will be accurate - if set to 1, the aimer will just point directly at the target.",
				0.0f, 1.0f) );
		
		m_applyToBarrel = new EditHelp.BoolPropertyInterface(
				"Apply aim to barrel",
				"Whether or not apply the aim directly to the gun object",
				"TRUE: The gun object will be immediately aimed to hit the target",
				"");
		m_editHelp.AddPropertyInterface( "m_applyAimToBarrel", m_applyToBarrel );
		
		m_maxIterations = new EditHelp.PropertyInterface(
				"Maximum Iterations",
				"");
		m_editHelp.AddPropertyInterface( "m_maximumIterations", m_maxIterations );
		
		m_maximumInaccuracy = new EditHelp.PropertyInterface(
				"Maximum Inaccuracy",
				"");
		m_editHelp.AddPropertyInterface( "m_maximumInaccuracy", m_maximumInaccuracy );
		
		/*
		m_editHelp.AddPropertyInterface( 
			"m_inaccuracyAngle", new EditHelp.SliderPropertyInterface(
				"Angle of inaccuracy",
				"If set to a value of greater than zero, the aim direction will be offset by a random amount up to this angle.\n" +
				"Good to use if you don't want your turrets to be too accurate.\n" +
				"0 = deadly accurate, 45 = can barely point in the right direction.",
				0, 90.0f) );
				*/
	}
	
	public override void OnInspectorGUI()
	{
		Aimer aimer = target as Aimer;
		/*
		PrefabShooter shooter = aimer.GetComponent<PrefabShooter>();
		m_gunObject.Enabled = ((shooter  == null) || !shooter.enabled);
		m_gunObject.HelpText = m_gunObject.Enabled 
			? "This is the object that will be aimed to point in the firing direction.\n" +
				"If this is left blank, the parent GameObject will be used."
			: "This field is currently disabled as the firing object is being set by the PrefabShooter also on this object.\n" +
				"To change this field, either disable or remove the PrefabShooter from the gameobject.";
				*/
		
		Limiter limits = aimer.GetComponent<Limiter>();
		m_applyToBarrel.Enabled = ((limits == null) || !limits.enabled);
		if (aimer.m_applyAimToBarrel && !m_applyToBarrel.Enabled)
		{
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			Undo.RegisterUndo(aimer, "Apply to barrel " + aimer.name);
#else
			Undo.RecordObject(aimer, "Apply to barrel " + aimer.name);
#endif
			aimer.m_applyAimToBarrel = false;
		}
		aimer.m_applyAimToBarrel &= m_applyToBarrel.Enabled;
		m_applyToBarrel.HelpText = m_applyToBarrel.Enabled
			? "Whether or not apply the aim directly to the gun object"
			: "This field is currently disabled as it is being set by the AimerLimits component also on this object.\n" +
				"To change this field, wither disable or remove the AimerLimits from the gameobject.";
		m_applyToBarrel.FalseHelp = m_applyToBarrel.Enabled
			? "FALSE: The gun object will not be aimed - it is up to the user (or another script) to access the AimDirection property to do the aiming."
			: "";
		
		m_maxIterations.Enabled = ((aimer.m_targetType != Aimer.TargetPredictionType.Linear) || (aimer.m_projectileType != Aimer.ProjectilePredictionType.Linear));
		m_maxIterations.HelpText = m_maxIterations.Enabled
			? "The maximum number of iterations the solver will take to aim."
			: "This value is not used when both target and projectile predictor types are linear, as a faster analytic method is used to aim.";
		
		m_maximumInaccuracy.Enabled = m_maxIterations.Enabled;
		m_maximumInaccuracy.HelpText = m_maximumInaccuracy.Enabled
			? "How close to the centre of the target the iterative solver has to get."
			: "This value is not used when both target and projectile predictor types are linear, as a faster analytic method is used to aim.";
		
		m_editHelp.EditProperties( aimer );
		
		if (Application.isPlaying)
		{
			bool enabled = GUI.enabled;
			GUI.enabled = false;
			EditorGUILayout.IntField("DEBUG: Iterations used", aimer.m_iterationsUsed);
			GUI.enabled = enabled;
		}
		
	}
		
	private EditHelp m_editHelp = new EditHelp();
	private EditHelp.PropertyInterface m_gunObject;
	private EditHelp.PropertyInterface m_swivelObject;
	private EditHelp.BoolPropertyInterface m_applyToBarrel;
	private EditHelp.PropertyInterface m_bulletSpeed;
	private EditHelp.PropertyInterface m_maxIterations;
	private EditHelp.PropertyInterface m_maximumInaccuracy;
	
}

