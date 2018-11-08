// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;


public class InstantProjectilePredictor : ProjectilePredictor
{
	
	public override Vector3 PredictPosition(Vector3 aimAt, float flightDuration)
	{
		return aimAt;
	}
	
	public override float EstimateTime(Vector3 aimAt)
	{
		return 0;
	}
	
	public override Vector3 Aim (Vector3 aimAt)
	{
		return (aimAt - transform.position).normalized;
	}
}

