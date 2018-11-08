// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LinearTargetTracker : TargetTracker
{
	public override Transform Target
	{
		get
		{
			return m_target;
		}
		set
		{
			if (m_target != value)
			{
				m_target = value;
				m_validCount = -1;
			}
		}
	}

	public override void UpdateTracker(float timeStep)
	{	
		Rigidbody targetRb = m_target.GetComponent<Rigidbody>();
		if (targetRb != null)
		{
			m_velocity = targetRb.velocity;
		}
		else
		{
			m_velocity = (m_target.position - m_previousPosition) / timeStep;
			m_previousPosition = m_target.position;
		}
		m_validCount++;
	}
	
	public override bool IsValidAim()
	{
		return m_validCount > 0;
	}
	

	public override Vector3 PredictPosition(float secondsInFuture)
	{
		return m_target.position + (m_velocity * secondsInFuture );
	}
	
	private Vector3 m_previousPosition;
	private int m_validCount;
}

