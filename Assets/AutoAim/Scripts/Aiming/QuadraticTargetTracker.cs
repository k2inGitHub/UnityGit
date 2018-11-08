// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class QuadraticTargetTracker : TargetTracker
{

	

	public override void UpdateTracker(float timeStep)
	{
		m_velocity = (m_target.position - m_previousPosition) / timeStep;
		m_velocity = m_target.GetComponent<Rigidbody>().velocity;
		m_previousPosition = m_target.position;
	}


	public override Vector3 PredictPosition(float secondsInFuture)
	{
		return m_target.position + (m_velocity * secondsInFuture) + Physics.gravity * secondsInFuture * secondsInFuture * 0.5f;
	}
	
	private Vector3 m_previousPosition;
}

