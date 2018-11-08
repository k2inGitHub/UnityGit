// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;



[CustomEditor(typeof(Targeter))]
public class TargeterEditor : Editor
{

	public override void OnInspectorGUI()
	{
		Targeter targeter = target as Targeter;
		
		Limiter limits = targeter.GetComponent<Limiter>();
		m_switchIfInvalid.Enabled = (limits != null) && limits.enabled;
		
		m_editHelp.EditProperties( targeter );

		
	}
	
	public void OnEnable()
	{
		m_editHelp = new EditHelp();
		
		m_editHelp.AddPropertyInterface( 
			"m_targetTag", new EditHelp.TagPropertyInterface(
				"Tag tag", 
				"The tags used to identify possible targets.") );
		
		m_editHelp.AddPropertyInterface( 
			"m_switchToClosest", new EditHelp.BoolPropertyInterface(
				"Switch to closest",
				"Whether or not to switch to another target if it is closer than the current target.",
				"TRUE: Turret will continually switch to the closest valid target.",
				"FALSE: Turret will continue to track current target even if another one comes closer.") );
		
		m_switchIfInvalid = new EditHelp.BoolPropertyInterface(
				"Switch if invalid",
				"Whether or not to switch to another target if the current one goes out of range (only valid with an AimerLimits component).",
				"TRUE: Turret will switch to another target if it cannot track the current one.",
				"FALSE: Turret will continue to track current target even if it goes out of bounds.");
		m_editHelp.AddPropertyInterface( "m_switchOnInvalid", m_switchIfInvalid );
		
			
	}

	
	private EditHelp m_editHelp;
	private EditHelp.BoolPropertyInterface m_switchIfInvalid;
}
