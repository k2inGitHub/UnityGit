// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class PlayParticleSystem : MonoBehaviour
{
	public ParticleSystem[] m_particleSystems;
	
	void PlayParticles()
	{
		foreach (ParticleSystem particle in m_particleSystems)
		{
			particle.Play();
		}
	}
}
