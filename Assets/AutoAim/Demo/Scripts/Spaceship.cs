// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class Spaceship : MonoBehaviour
{
	public float m_speed = 10.0f;
	public float m_rotationSpeed = 10.0f;
	public float m_bankScale = 1.0f;
	public GameObject m_modelRoot;

	// Use this for initialization
	void Start ()
	{
		float spacing = 30;
		float size = spacing - 2;
		
		m_buttons = new Rect[4];
		m_buttons[0] = new Rect(Screen.width - spacing * 2, Screen.height - spacing * 2, size, size);
		m_buttons[1] = new Rect(Screen.width - spacing * 3, Screen.height - spacing, size, size);
		m_buttons[2] = new Rect(Screen.width - spacing * 2, Screen.height - spacing, size, size);
		m_buttons[3] = new Rect(Screen.width - spacing, Screen.height - spacing, size, size);
		m_centre = new Vector3(Screen.width * 0.7f, Screen.height * 0.3f, 0);
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		float horiz = Input.GetAxis("Horizontal");
		float vert = Input.GetAxis("Vertical");
		
		
		if (Input.GetMouseButton(0))
		{
			Vector3 control = Input.mousePosition - m_centre;
			if (control.magnitude > controlWidth)
			{
				control = control.normalized * controlWidth;
			}
			horiz = control.x / controlWidth;
			vert = control.y / controlWidth;

		}
		
		transform.position += transform.forward * Time.fixedDeltaTime * m_speed;
		transform.Rotate(Vector3.up, m_rotationSpeed * horiz * Time.fixedDeltaTime);
		m_modelRoot.transform.localEulerAngles = new Vector3(0, 0, horiz * m_bankScale);
		transform.Rotate(Vector3.right, m_rotationSpeed * vert * Time.fixedDeltaTime);
	}
	

	void OnGUI ()
	{
		
		GUI.Label(m_buttons[0], "W", GUI.skin.box);
		GUI.Label(m_buttons[1], "A", GUI.skin.box);
		GUI.Label(m_buttons[2], "S", GUI.skin.box);
		GUI.Label(m_buttons[3], "D", GUI.skin.box);
		
		if (GUI.Button(new Rect(0, 0, 60, 30), "QUIT"))
		{
			Application.Quit();
		}
		
		if (GUI.Button(new Rect(0, Screen.height - 30, 60, 30), "Reset"))
		{
			transform.position = new Vector3(0, 10.0f, 0);
			transform.rotation = Quaternion.identity;
		}
	}

	private float controlWidth = 50;
	private Rect[] m_buttons;
	private Vector3 m_centre;
}
