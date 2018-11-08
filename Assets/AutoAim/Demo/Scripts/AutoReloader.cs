// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

// A little helper class to track amount of ammo, and force a reload animation when it's out of it.

public class AutoReloader : MonoBehaviour
{
	public int m_ammoCapacity;
	public Animation m_reloadAnimation;

	// Use this for initialization
	void Start ()
	{
		m_remainingAmmo = m_ammoCapacity;
		m_reloading = false;
		m_shooter = GetComponent<PrefabShooter>();
		m_aimer = GetComponent<Aimer>();
	}
	
	void OnShoot()
	{
		m_remainingAmmo--;
		
		if (m_remainingAmmo <= 0)
		{
			m_reloading = true;
		
			m_reloadAnimation.Play();
			Invoke("FinishedReloading", m_reloadAnimation.clip.length);
			m_shooter.enabled = false;
			m_aimer.enabled = false;
		}
		
	}
	
	void OnGUI()
	{
		if (m_reloading)
		{
			GUILayout.Label("Reloading");
		}
		else
		{
			GUILayout.Label("Remaining Ammo: " + m_remainingAmmo.ToString());
		}
	}
	
	void FinishedReloading()
	{
		m_reloading = false;
		m_shooter.enabled = true;
		m_aimer.enabled = true;
		m_remainingAmmo = m_ammoCapacity;
	}
	
	private int m_remainingAmmo;
	private bool m_reloading = false;
	private PrefabShooter m_shooter;
	private Aimer m_aimer;
}
