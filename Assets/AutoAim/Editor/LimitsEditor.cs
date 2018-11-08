// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(Limiter))]
public class LimitsEditor : Editor
{

	public void OnSceneGUI()
	{
		// make sure we detect any changes in the object
		GUI.changed = false;
		
		Limiter limits = target as Limiter;
		
		// Don't show limits if the node is disabled.
		if (!limits.enabled)
			return;
		
		float minRange = limits.m_minimumRange;
		float maxRange = limits.m_maximumRange;
		
		if (!limits.m_limitRange)
		{
			Renderer[] renderObjects = limits.GetComponentsInChildren<Renderer>();
			Bounds bounds = renderObjects[0].bounds;
			for(int i = 1; i < renderObjects.Length; ++i)
			{
				bounds.Encapsulate(renderObjects[i].bounds);
			}
			
			minRange = bounds.size.magnitude * 1.5f;
			maxRange = minRange * 2.5f;
			
		}
		
		Matrix4x4 handlesMatrix = Handles.matrix;
		Handles.matrix = limits.transform.localToWorldMatrix;
		
		// ignore scaling on the matrix;
		Vector3 scale = limits.transform.localScale;
		Vector3 invScale = new Vector3(1.0f / scale.x, 1.0f / scale.y, 1.0f / scale.z);
		Handles.matrix = Handles.matrix * Matrix4x4.Scale(invScale);
		
		
		Quaternion leftRotation = Quaternion.Euler(0.0f, limits.m_minimumHorizontalAngle, 0.0f);
		Quaternion rightRotation = Quaternion.Euler(0.0f, limits.m_maximumHorizontalAngle, 0.0f);
		Quaternion downRotation = Quaternion.Euler(-limits.m_minimumVerticalAngle, 0.0f, 0.0f);
		Quaternion upRotation = Quaternion.Euler(-limits.m_maximumVerticalAngle, 0.0f, 0.0f);
		Quaternion midRotoation = Quaternion.Euler((limits.m_maximumVerticalAngle +limits.m_minimumVerticalAngle) * -0.5f , 0.0f, 0.0f);
		
		Vector3 leftNormal = leftRotation * Vector3.right;
		Vector3 rightNormal = rightRotation * Vector3.right;
		
		Vector3 tm = upRotation * Vector3.forward;
		Vector3 tl = leftRotation * tm;
		Vector3 tr = rightRotation * tm;
		Vector3 bm = downRotation * Vector3.forward;
		Vector3 bl = leftRotation * bm;
		Vector3 br = rightRotation * bm;
		
		Vector3 tc = Vector3.up * Mathf.Sin(limits.m_maximumVerticalAngle * Mathf.Deg2Rad);
		Vector3 ts = leftRotation * Vector3.forward;
		
		Vector3 bc = Vector3.up * Mathf.Sin(limits.m_minimumVerticalAngle * Mathf.Deg2Rad);
		
		
		float horizArc = limits.m_maximumHorizontalAngle - limits.m_minimumHorizontalAngle;
		float vertArc = limits.m_maximumVerticalAngle - limits.m_minimumVerticalAngle;
		
		// Draw the arcs
		
		Handles.DrawWireArc(Vector3.zero, leftNormal, bl, -vertArc, maxRange);
		Handles.DrawWireArc(Vector3.zero, rightNormal, br, -vertArc, maxRange);
		
		Handles.DrawWireArc(Vector3.zero + tc * maxRange
			, Vector3.up
			, ts
			, horizArc
			, maxRange * Mathf.Cos(limits.m_maximumVerticalAngle * Mathf.Deg2Rad) );
		Handles.DrawWireArc(Vector3.zero + bc * maxRange
			, Vector3.up
			, ts
			, horizArc
			, maxRange * Mathf.Cos(limits.m_minimumVerticalAngle * Mathf.Deg2Rad) );
		
		
		Handles.DrawWireArc(Vector3.zero, leftNormal, bl, -vertArc, minRange);
		Handles.DrawWireArc(Vector3.zero, rightNormal, br, -vertArc, minRange);
		
		Handles.DrawWireArc(Vector3.zero + tc * minRange
			, Vector3.up
			, ts
			, horizArc
			, minRange * Mathf.Cos(limits.m_maximumVerticalAngle * Mathf.Deg2Rad) );
		Handles.DrawWireArc(Vector3.zero + bc * minRange
			, Vector3.up
			, ts
			, horizArc
			, minRange * Mathf.Cos(limits.m_minimumVerticalAngle * Mathf.Deg2Rad) );
		
		Handles.DrawLine(minRange * tl, maxRange * tl);
		Handles.DrawLine(minRange * bl, maxRange * bl);
		Handles.DrawLine(minRange * tr, maxRange * tr);
		Handles.DrawLine(minRange * br, maxRange * br);
		
		if (limits.m_limitRange)
		{
			Handles.DrawWireArc(Vector3.zero, Vector3.right, bm, -vertArc, minRange);
			Handles.DrawWireArc(Vector3.zero, Vector3.right, bm, -vertArc, maxRange);
			
			// Range sliders
		
			Vector3 centre = Quaternion.Euler((limits.m_minimumVerticalAngle + limits.m_maximumVerticalAngle) * -0.5f, 0.0f, 0.0f) * Vector3.forward;
			limits.m_maximumRange = Mathf.Max(Handles.Slider(limits.m_maximumRange * centre, centre).magnitude, limits.m_minimumRange);
			limits.m_minimumRange = Mathf.Min(Handles.Slider(limits.m_minimumRange * centre, centre).magnitude, limits.m_maximumRange);		
		}
		
		// angle sliders
		
		float midRange = (minRange + maxRange) * 0.5f;
		Vector3 horizCentre = Vector3.Dot(midRotoation * (Vector3.forward * midRange), Vector3.up ) * Vector3.up;
		
		StoreOriginalAngles( limits );
		
		limits.m_minimumHorizontalAngle = NewHandles.AngleControl(horizCentre, Vector3.up, Vector3.forward, limits.m_minimumHorizontalAngle, midRange, NewHandles.DoubleArrowCap, true);
		limits.m_maximumHorizontalAngle = NewHandles.AngleControl(horizCentre, Vector3.up, Vector3.forward, limits.m_maximumHorizontalAngle, midRange, NewHandles.DoubleArrowCap, false);
		
		limits.m_minimumVerticalAngle = -NewHandles.AngleControl(Vector3.zero, Vector3.right, Vector3.forward, -limits.m_minimumVerticalAngle, maxRange, NewHandles.DoubleArrowCap, true);
		limits.m_maximumVerticalAngle = -NewHandles.AngleControl(Vector3.zero, Vector3.right, Vector3.forward, -limits.m_maximumVerticalAngle, maxRange, NewHandles.DoubleArrowCap, true);
		
		// Apply limits to the angles
		LimitAngles( limits );
		
		Handles.matrix = handlesMatrix;
		
		// report any changes in the object.
		if (GUI.changed)
		{
			EditorUtility.SetDirty(limits);
		}
	}
	
	
	
	public void StoreOriginalAngles( Limiter limits )
	{
		m_originalMinH = limits.m_minimumHorizontalAngle;
		m_originalMaxH = limits.m_maximumHorizontalAngle;
		m_originalMinV = limits.m_minimumVerticalAngle;
		m_originalMaxV = limits.m_maximumVerticalAngle;
	}
	
	private void LimitAngles( Limiter limits)
	{
		float minH = limits.m_minimumHorizontalAngle;
		float maxH = limits.m_maximumHorizontalAngle;
		float minV = limits.m_minimumVerticalAngle; 
		float maxV = limits.m_maximumVerticalAngle; 
		
		
		// left
		minH = Mathf.Min(minH, m_originalMaxH);
		// Support mirroring
		if (limits.editor_mirrorHorizontalAngles && (minH != m_originalMinH))
		{
			minH = Mathf.Min(0, minH);
			maxH = -minH;
		}
		
		
		//right
		maxH = Mathf.Max(m_originalMinH, maxH);
		// mirroring
		if (limits.editor_mirrorHorizontalAngles && (maxH != m_originalMaxH))
		{
			maxH = Mathf.Max(0, maxH);
			minH = -maxH;
		}
		
		
		//upper
		maxV = Mathf.Clamp(maxV, -90.0f, 90.0f);
		maxV = Mathf.Max(maxV, m_originalMinV);
		if (limits.editor_mirrorVerticalAngles && (maxV != m_originalMaxV))
		{
			minV = -maxV;
		}
		
		//lower
		minV = Mathf.Clamp(minV, -90.0f, 90.0f);
		minV = Mathf.Min(m_originalMaxV, minV);
		if (limits.editor_mirrorVerticalAngles && (minV != m_originalMinV))
		{
			maxV = -minV;
		}
		
		limits.m_minimumHorizontalAngle = minH;
		limits.m_maximumHorizontalAngle = maxH;
		limits.m_minimumVerticalAngle = minV;
		limits.m_maximumVerticalAngle = maxV;
	}
	
	public void OnEnable()
	{
		m_editHelp = new EditHelp();
		
		m_editHelp.AddPropertyInterface( 
			"m_minimumHorizontalAngle", new EditHelp.SliderPropertyInterface(
				"Maximum horizontal angle", 
				"Maximum (right hand) limit for aiming.",
				-180.0f, 180.0f) );
		
		m_editHelp.AddPropertyInterface( 
			"m_maximumHorizontalAngle", new EditHelp.SliderPropertyInterface(
				"Maximum horizontal angle",
				"Maximum (right hand) limit for aiming.",
				-180.0f, 180.0f) );
			
		
		m_editHelp.AddPropertyInterface( 
			"editor_mirrorHorizontalAngles", new EditHelp.BoolPropertyInterface(
				"Mirror the horizintal limits",
				"Whether or not to mirror the horizontal limits",
				"TRUE: limits are mirrored around zero degrees.",
				"FALSE: limits can be independantly altered.") );
		
		m_editHelp.AddPropertyInterface( 
			"m_minimumVerticalAngle", new EditHelp.SliderPropertyInterface(
				"Minimum vertical angle", 
				"Maximum (upper) limit for aiming.",
				-90.0f, 90.0f) );
		
		m_editHelp.AddPropertyInterface( 
			"m_maximumVerticalAngle", new EditHelp.SliderPropertyInterface(
				"Maximum vertical angle",
				"Maximum (lower) limit for aiming.",
				-90.0f, 90.0f) );
		
		m_editHelp.AddPropertyInterface( 
			"editor_mirrorVerticalAngles", new EditHelp.BoolPropertyInterface(
				"Mirror the vertical limits",
				"Whether or not to mirror the vertical limits",
				"TRUE: limits are mirrored around zero degrees.",
				"FALSE: limits can be independantly altered.") );
		
		m_editHelp.AddPropertyInterface( 
			"m_limitRange", new EditHelp.BoolPropertyInterface(
				"Limit the range",
				"Whether or not to limit the range of the turret",
				"TRUE: Turret range will be limited.",
				"FALSE: Turret range is unlimited.") );
		
		m_minRange = new EditHelp.PropertyInterface(
				"Minimum Range",
				"Closest an object can be to be aimed at");
		m_editHelp.AddPropertyInterface( "m_minimumRange", m_minRange ) ;
		
		m_maxRange = new EditHelp.PropertyInterface(
				"Maximum Range",
				"Furthest an object can be to be aimed at");
		m_editHelp.AddPropertyInterface( "m_maximumRange", m_maxRange ) ;
		
		m_editHelp.AddPropertyInterface( 
			"m_limitTurnSpeed", new EditHelp.BoolPropertyInterface(
				"Limit the turn speed",
				"Whether or not to limit turning speed of the turret",
				"TRUE: Turret turn speed will be limited.",
				"FALSE: Turret turn speed will not be limited.") );
		
		m_turnSpeed = new EditHelp.PropertyInterface(
				"Maximum Turn Speed",
				"Maximum speed the turret can turn in degrees per second.\nFor example: 180 is fast. 5 is slow.");
		
		m_editHelp.AddPropertyInterface( "m_maxTurnSpeed", m_turnSpeed ) ;
		
		m_editHelp.AddPropertyInterface( 
			"m_returnToCentre", new EditHelp.BoolPropertyInterface(
				"Return to Centre",
				"Whether or not to return the turret to the centre when it has no target",
				"TRUE: Turret will return to centre.",
				"FALSE: Turret will stay at its last position.") );
	}
	
	public override void OnInspectorGUI()
	{
		Limiter limits = target as Limiter;
		
		if (!limits.GetComponent<Aimer>())
		{
			EditorGUILayout.HelpBox("Warning: Requries Aimer Component", MessageType.Warning);
			if (GUILayout.Button("Press to add an Aimer component"))
			{
				limits.gameObject.AddComponent<Aimer>();
			}
		}
		
		m_minRange.Enabled = limits.m_limitRange;
		m_maxRange.Enabled = limits.m_limitRange;
		
		m_turnSpeed.Enabled = limits.m_limitTurnSpeed;
		
		StoreOriginalAngles( limits );
		
		m_editHelp.EditProperties( limits );
		
		LimitAngles( limits );
		
	}
	
	
	private float m_originalMinH = 0.0f;
	private float m_originalMaxH = 1.0f;
	private float m_originalMinV = 0.0f;
	private float m_originalMaxV = 1.0f;
	
	private EditHelp m_editHelp;
	private EditHelp.PropertyInterface m_minRange;
	private EditHelp.PropertyInterface m_maxRange;
	
	private EditHelp.PropertyInterface m_turnSpeed;
	
}

