// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;


public class IterativeAimer
{
	public TargetTracker m_targetTracker;
	public ProjectilePredictor m_projectilePredictor;
	public float m_timeToHit;
	public int m_iterationsTaken;
	public float m_maximumInaccuracy = 0.1f;
	
	public void Start()
	{
		m_estimationError = 1.0f;
	}
	
	public float GetSqrError(float time)
	{
		// calculate difference in position
		Vector3 targetPos = m_targetTracker.PredictPosition(time);
		Vector3 bulletPos = m_projectilePredictor.PredictPosition(targetPos, time);
		
		Vector3 vecToBullet = bulletPos - targetPos;
		float vecLength = vecToBullet.sqrMagnitude;
		
		return vecLength;
	}
	
	public Vector3 IterativePredict(int iterations, out Vector3 predictedPosition, out float time)
	{
		float squaredInaccuracy = m_maximumInaccuracy * m_maximumInaccuracy;
		
		// update the tracker
		m_targetTracker.UpdateTracker(Time.fixedDeltaTime);
		
		predictedPosition = Vector3.zero;
		time = 0;
		
		// estimate initial time
		float estimatedTime = m_projectilePredictor.EstimateTime(m_targetTracker.Target.position);
		float originalEstimate = estimatedTime;
		
		// adjust from previous frame's estimate
		estimatedTime *= m_estimationError;
	
		// Get the error
		float sqrError = GetSqrError(estimatedTime);
		
		// estimate a maximum time error
		float timediff = (Mathf.Sqrt(sqrError) / (m_projectilePredictor.m_projectileSpeed + m_targetTracker.Velocity.magnitude)) * 1.2f;

		for (m_iterationsTaken = 0; m_iterationsTaken < iterations; ++m_iterationsTaken)
		{
			//m_aimanaughts[i].transform.position = m_targetTracker.PredictPosition(estimatedTime);
			
			// break out if we're within a small distance
			if (sqrError <= squaredInaccuracy)
			{
				m_iterationsTaken++;
				break;
			}
			
			float addErr = GetSqrError(estimatedTime + timediff);
			float subErr = GetSqrError(estimatedTime - timediff);
			
			if ((sqrError > addErr) || (sqrError > subErr))
			{
				if (subErr < addErr)
				{
					estimatedTime -= timediff;
					sqrError = subErr;
				}
				else
				{
					estimatedTime += timediff;
					sqrError = addErr;
				}
			}
			//estimatedTime = (Mathf.Floor(estimatedTime / Time.fixedDeltaTime) * Time.fixedDeltaTime);
			timediff /= 2.0f;
		}
		
		//Debug.Log("Estimated Error : " + sqrError.ToString());
		
		
		if (originalEstimate != 0.0f)
		{
			m_estimationError = estimatedTime / originalEstimate;
		}
		else
		{
			m_estimationError = 0.0f;
		}
		
		//Debug.Log("Estimation Error : " + m_estimationError.ToString());
		
		m_timeToHit = estimatedTime;
		
		Vector3 targetPos = m_targetTracker.PredictPosition(estimatedTime);
		
		predictedPosition = targetPos;
		time = estimatedTime;
		
		return m_projectilePredictor.Aim(targetPos);
	}
	
	private GameObject[] m_aimanaughts;
	private float m_estimationError;
	
}

