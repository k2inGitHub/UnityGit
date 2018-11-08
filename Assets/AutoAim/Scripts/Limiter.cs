// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("AutoAim/Limiter")]
public class Limiter : MonoBehaviour
{
	public float m_minimumHorizontalAngle = -45;
	public float m_maximumHorizontalAngle = 45;
	public float m_minimumVerticalAngle = 0;
	public float m_maximumVerticalAngle = 60;
	public bool m_limitRange = false;
	public float m_minimumRange = 1;
	public float m_maximumRange = 50;
	public bool m_limitTurnSpeed = false;
	public float m_maxTurnSpeed = 45;
	public bool m_returnToCentre = false;
	
	public bool editor_mirrorHorizontalAngles = false;
	public bool editor_mirrorVerticalAngles = false;

	// Use this for initialization
	void Start ()
	{
		m_aimer = GetComponent<Aimer>();
		m_initialRotation = m_aimer.m_gunObject.transform.rotation;
	}
	
	
	public bool DetermineAimingAngles(Vector3 direction, out float horizAngle, out float vertAngle)
	{
		float fwd = Vector3.Dot(direction, transform.forward);
		float up = Vector3.Dot(direction, transform.up);
		float right = Vector3.Dot(direction, transform.right);
		float combined = Mathf.Sqrt(fwd * fwd + right * right);
		
		float initialHorizAngle = Mathf.Atan2(right, fwd) * Mathf.Rad2Deg;
		float initialVertAngle = Mathf.Atan2(up, combined) * Mathf.Rad2Deg;
		
		horizAngle = Mathf.Clamp(initialHorizAngle, m_minimumHorizontalAngle, m_maximumHorizontalAngle);
		vertAngle = Mathf.Clamp(initialVertAngle, m_minimumVerticalAngle, m_maximumVerticalAngle);
		
		return (horizAngle == initialHorizAngle) && (vertAngle == initialVertAngle);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		float horizAngle;
		float vertAngle;
		
		Quaternion targetAngle = m_initialRotation;
		Quaternion startAngle = m_aimer.m_gunObject.transform.rotation;
		
		if (!m_returnToCentre || (m_aimer.m_targetObject != null))
		{
			DetermineAimingAngles(m_aimer.AimDirection, out horizAngle, out vertAngle);
			targetAngle = transform.rotation * Quaternion.Euler(-vertAngle, horizAngle, 0.0f);
		}
		
		float maxAngleThisFrame = m_maxTurnSpeed * Time.fixedDeltaTime;
		
		// aim...!
		if (m_limitTurnSpeed)
		{
			m_aimer.ApplyRotation(Quaternion.RotateTowards(startAngle, targetAngle, maxAngleThisFrame));
		}
		else
		{
			m_aimer.ApplyRotation(targetAngle);
		}
	}
	
	public bool ViableTarget(Vector3 position)
	{
		if ((m_aimer == null) || (m_aimer.m_gunObject == null)) 
			return false;
		
		Vector3 direction = (position - m_aimer.m_gunObject.transform.position).normalized;
		
		float h;
		float v;
		
		if (!DetermineAimingAngles(direction, out h, out v))
		{
			return false;
		}
	
		if (!m_limitRange)
		{
			return true;
		}
		
		float distance = Vector3.Distance(m_aimer.m_gunObject.transform.position, position);
		
		return ((distance >= m_minimumRange) && (distance <= m_maximumRange));
	}
	
	private Aimer m_aimer;
	private Quaternion m_initialRotation;
}

