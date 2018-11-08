// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;


public class QuadraticProjectilePredictor : ProjectilePredictor
{
	
	public override Vector3 Aim (Vector3 aimAt)
	{
		Vector3 diff = aimAt - transform.position;
		
		Vector3 up = -Physics.gravity.normalized;
		Vector3 right = Vector3.Cross(up, diff.normalized).normalized;
		Vector3 forward = Vector3.Cross(right, up);
		float y = Vector3.Dot(up, diff);
		diff.y = 0;
		float x = Vector3.Dot(forward, diff);
		
		float g = Physics.gravity.magnitude;
		float v= m_projectileSpeed;
		
		float root = Mathf.Pow( m_projectileSpeed, 4) - g * (g * x * x + 2 * y * v * v);
		if (root > 0)
		{
			float squaredRoot = Mathf.Sqrt(root);
			
			float firstVariant = Mathf.Atan((v * v - squaredRoot) / (g * x));
			float secondVariant = Mathf.Atan((v * v + squaredRoot) / (g * x));
		
		
			float angle = Mathf.Min(firstVariant, secondVariant);
			
			Vector3 direction = (diff.normalized * Mathf.Cos(angle) + Mathf.Sin(angle) * Vector3.up).normalized;
			
			return direction;
		}
		
		return Vector3.zero;
	}
	
	public override Vector3 PredictPosition(Vector3 aimAt, float flightDuration)
	{
		Vector3 direction = Aim(aimAt);
			
		Vector3 position = direction * m_projectileSpeed * flightDuration + 0.5f * Physics.gravity * flightDuration * flightDuration;
		
		return position + transform.position;
	}
	
	public override float EstimateTime(Vector3 aimAt)
	{
		Vector3 diff = aimAt - transform.position;
		return diff.magnitude / m_projectileSpeed;
	}
	
	
}

