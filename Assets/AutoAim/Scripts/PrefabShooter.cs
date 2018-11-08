// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("AutoAim/Prefab Shooter")]
public class PrefabShooter : MonoBehaviour
{
	//public float m_bulletVelocity;
	public GameObject m_bulletPrefab;
	public float m_firingPause = 0.5f;
	
	public bool m_autoFire = true;
	public float m_firingCone = 0.0f;
	public float m_bulletLifetime = 2.5f;
	public bool m_enableGravity = false;
	public Animation m_firingAnimation;
	public string m_firingMessage;
	public bool m_checkForObstacles = false;
	public LayerMask m_obstacleLayers = -1;
	
	private static readonly float cosOneDegree = Mathf.Cos(1.0f * Mathf.Deg2Rad);

	[System.Serializable]
	public class ExitPoint
	{
		
		public Animation firingAnimation;
		public Vector3 firingOffset;
		internal Vector3 calculatedStartPos;
	}
	
	public ExitPoint[] m_exitPoints;// = new ExitPoint[0];
	

	void Start ()
	{
		m_lastShot = 0;
		
		m_autoFire = true;
		
		m_aimer = GetComponent<Aimer>();
		m_limiter = GetComponent<Limiter>();
		
		if (m_aimer && m_aimer.enabled)
		{
			m_shotTransform = m_aimer.m_gunObject.transform;
			m_enableGravity = (m_aimer.m_projectileType == Aimer.ProjectilePredictionType.Parabolic);
		}
		
		m_fireIndex = 0;
		
		foreach ( ExitPoint point in m_exitPoints)
		{
			point.calculatedStartPos = point.firingOffset;//m_shotTransform.InverseTransformPoint(point.firingOffset);
		}
		
	}
	
	void Fire()
	{
		if (!m_shotTransform)
		{
			return;
		}
		
		// Check here for obstacles, and early out if necessary.
		if (m_checkForObstacles)
		{
			// TODO : handle case for parabolic shots

			// make a rough guess for the predicted position of the target...
			float distanceToTarget = (m_aimer.m_targetObject.transform.position - m_shotTransform.position).magnitude;
			Vector3 estimatedTargetPos = (m_shotTransform.forward * distanceToTarget) + m_shotTransform.position;
			
			RaycastHit hitInfo;
			if (Physics.Linecast(m_shotTransform.position, estimatedTargetPos, out hitInfo, m_obstacleLayers))
			{
				if (!hitInfo.collider.transform.IsChildOf(m_aimer.m_targetObject.transform))
				{
					// Don't shoot if it's not the target tag
					return;
				}
			}
		}
		
		Quaternion rotation = m_shotTransform.rotation;
		
		if (m_firingCone != 0.0f)
		{
			float xAngle = m_firingCone * Mathf.Sqrt(UnityEngine.Random.Range(0.0f, 1.0f));
			Quaternion xRotation = Quaternion.Euler(xAngle , 0.0f, 0.0f);
			
			float zAngle = UnityEngine.Random.Range(0, 360.0f);
			Quaternion zRotation = Quaternion.Euler(0.0f, 0.0f, zAngle);
			
			rotation = rotation * (zRotation * xRotation);
		}
		
		Vector3 startPos = m_shotTransform.position;
		
		if (m_exitPoints.Length > 0)
		{
			ExitPoint point = m_exitPoints[m_fireIndex];
			Vector3 offsetPos = point.calculatedStartPos;
			offsetPos.z = 0.0f;
			startPos = m_shotTransform.TransformPoint(offsetPos);
			
			if (point.firingAnimation)
			{
				point.firingAnimation.Play();
			}
			
			m_fireIndex = (m_fireIndex + 1) % m_exitPoints.Length;

			
		}
		
		
		GameObject bullet = GameObject.Instantiate(m_bulletPrefab, startPos, rotation ) as GameObject;
		Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
		bulletRb.AddRelativeForce(new Vector3(0, 0, m_aimer.m_projectileSpeed),ForceMode.VelocityChange);
		bulletRb.useGravity = m_enableGravity;
		
		Destroy(bullet, m_bulletLifetime);
		
		if (m_firingAnimation)
		{
			m_firingAnimation.Play();
		}
		
		if (m_firingMessage != "")
		{
			SendMessage(m_firingMessage, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void FixedUpdate ()
	{
		if (m_autoFire && m_aimer && m_aimer.m_targetObject && m_aimer.m_hasAimed)
		{
			if (m_limiter)
			{
				if (!m_limiter.ViableTarget(m_aimer.m_targetObject.transform.position))
				{
					return;
				}
				
				if (Vector3.Dot(m_aimer.AimDirection, m_shotTransform.forward) < cosOneDegree) // should make this user-settable? - set to 1 degree at the moment
				{
					return;
				}
			}
			if ((Time.time - m_lastShot) > m_firingPause)
			{
				m_lastShot = Time.time;
				Fire();
			}
		}
	}
	
	void OnEnable()
	{
		// Ensure we don't fire until the frame after we've been enabled
		m_lastShot = Time.time - m_firingPause + Time.fixedDeltaTime;
	}
	
	public float m_lastShot;
	private Aimer m_aimer;
	private Limiter m_limiter;
	private Transform m_shotTransform;
	private int m_fireIndex;
}

