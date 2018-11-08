// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class ArcShooter : MonoBehaviour
{
	public float speed;

	private Rigidbody m_rigidBody;

	void Start()
	{
		m_rigidBody = GetComponent<Rigidbody>();
	}
	
	void Update ()
	{
		if (transform.position.y < 0.0f)
		{
			transform.position = new Vector3(Random.Range(-40f, 40f), 5.0f, Random.Range(-40f, 40f));
			Vector3 direction = new Vector3(Random.Range(-1f, 1f), 2.0f, Random.Range(-1f, 1f)).normalized;
			
			m_rigidBody.velocity = direction * speed;
		}
	
	}
}
