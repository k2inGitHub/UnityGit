// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class SpawnExplosionOnHit : MonoBehaviour
{
	public GameObject m_explosion;

	// Update is called once per frame
	void OnTriggerEnter( Collider collider )
	{
		if (m_explosion)
		{
			GameObject explosion = GameObject.Instantiate(m_explosion, collider.transform.position, collider.transform.rotation ) as GameObject;
			explosion.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity * 0.5f;
			Destroy(gameObject);
			Destroy(explosion, 2.0f);
		}
	}
}
