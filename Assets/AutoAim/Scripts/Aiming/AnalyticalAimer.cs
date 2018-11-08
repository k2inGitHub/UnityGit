// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class AnalyticalAimer
{
	public GameObject m_target;
	public Transform m_transform;
	public float m_bulletSpeed;
	
	public void SetTarget( GameObject target)
	{
		if (target != m_target)
		{
			m_target = target;
			if (target)
			{
				m_lastPosition = target.transform.position;
			}
			m_validAim = -1;
		}
	}
	
	static public bool SolveQuadratic(out float first, out float second, float a, float b, float c)
	{
		float partialTop = b * b - 4.0f * a * c;
		if (partialTop < 0)
		{
			first = 0;
			second = 0;
			return false;
		}
		
		partialTop = Mathf.Sqrt(partialTop);
		float bottom = 2.0f * a;
		first = (-b + partialTop) / bottom;
		second = (-b - partialTop) / bottom;
		
		return true;
	}

	public Vector3 Predict(out Vector3 predictedPosition, out float time, out bool valid)
	{
		
		if (m_validAim < 0)
		{
			time = 0.0f;
			valid = false;
			m_lastPosition = m_target.transform.position;
		}
		else
		{
			valid = true;
		}
		m_validAim++;
		
		Vector3 velocity;
		Rigidbody targetRb = m_target.GetComponent<Rigidbody> ();
		if ((targetRb != null) && !targetRb.isKinematic)
		{
			velocity = targetRb.velocity;
		}
		else
		{
			velocity = (m_target.transform.position - m_lastPosition) / Time.fixedDeltaTime; // Hmmm....
			m_lastPosition = m_target.transform.position;
		}
		Vector3 diff = m_target.transform.position - m_transform.position;
		
		float a = velocity.sqrMagnitude - m_bulletSpeed * m_bulletSpeed;
		float b = 2.0f * (diff.x + diff.y + diff.z);
		float c = diff.sqrMagnitude;
		
		float first, second;
		if (SolveQuadratic(out first, out second, a, b, c))
		{
			time = first;
			if ((time < 0.0f) || ((second < time) && (second > 0.0f)))
			{
				time = second;
			}
			predictedPosition = m_target.transform.position + velocity * time;
			return (predictedPosition - m_transform.position).normalized;
		}
		// if we cannot solve, just give the current position
		time = 0.0f;
		predictedPosition = m_target.transform.position;
		return m_transform.forward;
	}
	
	private Vector3 m_lastPosition;
	private int m_validAim;
}
