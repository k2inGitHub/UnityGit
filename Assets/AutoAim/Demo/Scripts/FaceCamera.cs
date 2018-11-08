// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class FaceCamera : MonoBehaviour
{	
	// Update is called once per frame
	void Update ()
	{
		transform.LookAt(Camera.main.transform.position);
	}
}
