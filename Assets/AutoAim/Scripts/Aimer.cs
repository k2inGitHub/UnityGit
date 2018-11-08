// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("AutoAim/Aimer")]
public class Aimer : MonoBehaviour
{

	public enum TargetPredictionType
	{
		Linear = 0,
		Parabolic,
		Circular
	}
	
	public enum ProjectilePredictionType
	{
		Instant = 0,
		Linear,
		Parabolic
	}
	
	public TargetPredictionType m_targetType = TargetPredictionType.Linear;
	public ProjectilePredictionType m_projectileType;
	
	public float m_projectileSpeed = 100.0f;
	
	public GameObject m_targetObject;
	public GameObject m_gunObject;
	public GameObject m_optionalSwivel;
	public bool m_applyAimToBarrel = true;
	
	public Vector3 AimDirection
	{
		get { return m_aimDirection; }
	}
	
	
	public float m_maximumInaccuracy = 0.1f;
	
	public int m_maximumIterations = 10;
	public int m_iterationsUsed = 0;
	public float m_artificalStupidity = 0.0f;
	
	public bool m_hasAimed = false;
	
	void Start()
	{		
		if (!m_gunObject)
		{
			m_gunObject = gameObject;
		}
		
		if ((m_targetType == TargetPredictionType.Linear) && (m_projectileType == ProjectilePredictionType.Linear))
		{
			m_analyticalAimer = new AnalyticalAimer();
			m_analyticalAimer.m_target = m_targetObject;
			m_analyticalAimer.m_bulletSpeed = m_projectileSpeed;
			m_analyticalAimer.m_transform = m_gunObject.transform;
		}
		else
		{
			m_iterativeAimer = new IterativeAimer();
		
			switch( m_targetType )
			{
			case TargetPredictionType.Linear:
				m_iterativeAimer.m_targetTracker = new LinearTargetTracker();
				break;
			case TargetPredictionType.Parabolic:
				m_iterativeAimer.m_targetTracker = new QuadraticTargetTracker();
				break;
			case TargetPredictionType.Circular:
				CircularTargetTracker tracker =  new CircularTargetTracker();
				tracker.m_lineRenderer = GetComponent<LineRenderer>();
				m_iterativeAimer.m_targetTracker = tracker;
				break;
			}
			
				
			switch( m_projectileType )
			{
			case ProjectilePredictionType.Instant:
				m_iterativeAimer.m_projectilePredictor = new InstantProjectilePredictor();
				break;
			case ProjectilePredictionType.Linear:
				m_iterativeAimer.m_projectilePredictor = new LinearProjectilePredictor();
				break;
			case ProjectilePredictionType.Parabolic:
				m_iterativeAimer.m_projectilePredictor = new QuadraticProjectilePredictor();
				break;
			}
			m_iterativeAimer.m_projectilePredictor.transform = m_gunObject.transform;
			m_iterativeAimer.m_projectilePredictor.m_projectileSpeed = m_projectileSpeed;
			m_iterativeAimer.m_maximumInaccuracy = m_maximumInaccuracy;
			m_iterativeAimer.Start();
		}
		
		
		
	}
	
	
	
	//public GameObject m_aimPrefab;
	
	void FixedUpdate()
	{
		m_hasAimed = false;
		if (m_targetObject)
		{
			
			Vector3 targetPos;
			float time;
			
			
			if (m_iterativeAimer != null)
			{
				m_iterativeAimer.m_targetTracker.Target = m_targetObject.transform;
				Vector3 aimDirection = m_iterativeAimer.IterativePredict(m_maximumIterations, out targetPos, out time);
				
				if (!m_iterativeAimer.m_targetTracker.IsValidAim())
				{
					return;
				}
				m_aimDirection = aimDirection;
				
				m_iterationsUsed = m_iterativeAimer.m_iterationsTaken;
			}
			else
			{
				m_analyticalAimer.m_target = m_targetObject;
				bool valid = true;
				m_aimDirection = m_analyticalAimer.Predict(out targetPos, out time, out valid);
				if (!valid)
				{
					return;
				}
				m_iterationsUsed = 1;
			}
			
			if ( m_artificalStupidity != 0.0f )
			{
				// create a baseline target direction
				Vector3 stoopidDirection = (m_targetObject.transform.position - m_gunObject.transform.position).normalized;
				m_aimDirection = Vector3.Slerp(m_aimDirection, stoopidDirection, m_artificalStupidity);
			}
			
			
			if ( m_applyAimToBarrel )
			{
				Quaternion aimRotation = Quaternion.LookRotation(m_aimDirection, transform.up);
				ApplyRotation(aimRotation);
			}
			
			m_hasAimed = true;
			
		}
		else
		{
			if (m_iterativeAimer != null)
			{
				m_iterativeAimer.m_targetTracker.Target = null;
			}
			else
			{
				m_analyticalAimer.SetTarget(null);
			}
		}
	}
	
	public void ApplyRotation(Quaternion rotation)
	{
		
		if (m_optionalSwivel)
		{
			// Need to transform the rotation to the local transform.
			Vector3 localAimAt = m_optionalSwivel.transform.parent.worldToLocalMatrix.MultiplyVector(rotation * Vector3.forward);
			Quaternion localRotation = Quaternion.LookRotation(localAimAt, m_optionalSwivel.transform.up);
			
			m_optionalSwivel.transform.localRotation = Quaternion.Euler(0.0f, localRotation.eulerAngles.y, 0.0f);
			
			m_gunObject.transform.rotation = rotation;

			// Work out what angle the up vector of the gun barrel is out from the up vector of the swivel, and correct it.
			float angle = Mathf.Asin(Vector3.Dot(m_gunObject.transform.up, m_optionalSwivel.transform.right));
			m_gunObject.transform.Rotate(m_gunObject.transform.forward, angle);
		}
		else
		{
			m_gunObject.transform.rotation = rotation;
		}
	}

	private Vector3 m_aimDirection;
	private IterativeAimer m_iterativeAimer;
	private AnalyticalAimer m_analyticalAimer;

}

