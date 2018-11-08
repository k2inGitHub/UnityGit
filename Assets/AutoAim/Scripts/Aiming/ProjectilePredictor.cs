// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;


public abstract class ProjectilePredictor
{
	public Transform transform;
	
	public float m_projectileSpeed;
	
	public abstract Vector3 Aim(Vector3 aimAt);
	
	public abstract Vector3 PredictPosition(Vector3 aimAt, float flightDuration);
	
	public abstract float EstimateTime(Vector3 aimAt);
	
	
}

