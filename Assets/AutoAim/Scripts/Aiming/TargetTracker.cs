// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;


public abstract class TargetTracker
{
	public virtual Transform Target
	{
		get
		{
			return m_target;
		}
		set
		{
			m_target = value;
		}
	}
	
	public Vector3 Velocity
	{
		get { return m_velocity; }
	}
	
	public virtual bool IsValidAim()
	{
		return true;
	}
	

	public abstract void UpdateTracker(float timeStep);
	
	
	public abstract Vector3 PredictPosition(float secondsInFuture);
	
	protected  Vector3 m_velocity;
	protected Transform m_target;
}

