// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("AutoAim/Targeter")]
public class Targeter : MonoBehaviour
{
	public string m_targetTag = "enemy";
	public bool m_switchToClosest = true;
	public bool m_switchOnInvalid = true;
	
	
	public GameObject Target
	{
		get { return m_target; }
	}
	
	void Start()
	{
		m_aimer = GetComponent<Aimer>();
		m_limiter = GetComponent<Limiter>();
	}
	
	bool IsValidTarget(GameObject target)
	{
		if (m_limiter && m_limiter.enabled)
		{
			if (!m_limiter.ViableTarget(target.transform.position))
			{
				return false;
			}
		}
		return true;
	}
	
	
	void FixedUpdate()
	{
#if UNITY_3_5
		if ((m_target == null) || !m_target.active || m_switchToClosest || !IsValidTarget(m_target))
#else
		if ((m_target == null) || !m_target.activeInHierarchy || m_switchToClosest || !IsValidTarget(m_target))
#endif
		{
			GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(m_targetTag);
			float minDistance = Mathf.Infinity;
			
			m_target = null;
			
			foreach (var taggedObj in taggedObjects)
			{
				if (!IsValidTarget(taggedObj))
				{
					continue;
				}
				
				float sqrDistance = Vector3.SqrMagnitude(taggedObj.transform.position - transform.position);
				if (sqrDistance < minDistance)
				{
					minDistance = sqrDistance;
					m_target = taggedObj;
				}
			}
			
			if (m_aimer != null)
			{
				m_aimer.m_targetObject = m_target;
			}
		}
	}
	
	
	private GameObject m_target;
	private Aimer m_aimer;
	private Limiter m_limiter;
	
}

