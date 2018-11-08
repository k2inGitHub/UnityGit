// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;


public class LinearProjectilePredictor : ProjectilePredictor
{
	
	public override Vector3 PredictPosition(Vector3 aimAt, float flightDuration)
	{
		return ((aimAt - transform.position).normalized * m_projectileSpeed * flightDuration) + transform.position;
	}
	
	public override float EstimateTime(Vector3 aimAt)
	{
		Vector3 diff = aimAt - transform.position;
		return diff.magnitude / m_projectileSpeed;
	}
	
	public override Vector3 Aim (Vector3 aimAt)
	{
		return (aimAt - transform.position).normalized;
	}
}

