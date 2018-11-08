// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class FollowCamera : MonoBehaviour
{
	public Transform m_foreground;
	public Transform m_background;
	
	public float m_height;
	public float m_distance;
	
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 direction = m_background.position - m_foreground.position;
		transform.position = m_foreground.position + Vector3.up * m_height - m_distance * direction.normalized;
		if (transform.position.y < 0.0f)
		{
			transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
		}
		transform.LookAt((m_background.position + m_foreground.position) * 0.5f);
	}
}
