// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CircularTargetTracker : TargetTracker
{
	public LineRenderer m_lineRenderer;
	
	public override Transform Target
	{
		get
		{
			return m_target;
		}
		set
		{
			if (m_target != value)
			{
				m_target = value;
				m_validCount = -20;
			}
		}
	}
	
	public override bool IsValidAim()
	{
		return m_validCount > 0;
	}
	
	
	public CircularTargetTracker()
	{
		Vector3 pos = Vector3.zero;
		m_firstHalf = new  Queue<Vector3>();
		m_secondHalf = new Queue<Vector3>();
		if (m_target)
		{
			pos = m_target.position;
		}
		for (int i = 0; i < queueSize; ++i)
		{
			m_firstHalf.Enqueue(pos);
			m_secondHalf.Enqueue(pos);
		}
		m_validCount = -20;
	}
	
	public override void UpdateTracker (float timeStep)
	{
		// Work out circle of travel from previous points
		// NOTE: perhaps take points from more than 1 & 2 frames back?
		
		// update the positions
		// *UGH* 
		/*
		for (int i = 20; i > 0; --i)
		{
			m_circlePositions[i] = m_circlePositions[i - 1];
		}
		m_circlePositions[0] = m_target.position;
		*/
		
		m_validCount++;
		
		m_start = m_target.position;
		m_middle = m_firstHalf.Dequeue();
		m_end = m_secondHalf.Dequeue();
		
		
		
		// early out if the motion is roughly linear.
		Vector3 rayRight = (m_start - m_middle).normalized;
		m_direction = (m_start - m_lastPosition);
		m_speed = m_direction.magnitude / timeStep;
		Vector3 planeRight = (m_middle - m_end).normalized;
		
		if (Vector3.Dot(rayRight, planeRight) > 0.9999f)
		{
			m_isLinear = true;
			m_lastPosition = m_target.position;
			m_firstHalf.Enqueue(m_start);
			m_secondHalf.Enqueue(m_middle);
			return;
		}
		m_isLinear = false;
		
		
		// Work out plane that three points go through
		Plane circlePlane = new Plane(m_start, m_middle, m_end);
		Vector3 rayPoint = (m_start + m_middle) * 0.5f;
		
		Ray ray = new Ray(rayPoint, Vector3.Cross(rayRight, circlePlane.normal));
		
		Vector3 planePoint = (m_middle + m_end) * 0.5f;
		
		Plane plane = new Plane(planeRight, planePoint);
		
		float distance = 0;
		plane.Raycast(ray, out distance);
		
		m_circleCentre = ray.GetPoint(distance);
		m_circleNormal = circlePlane.normal;
		
		Quaternion rotation = Quaternion.AngleAxis(10, m_circleNormal);
		
		if (m_lineRenderer)
		{
			m_lineRenderer.SetVertexCount(36);
			Vector3 point = m_start - m_circleCentre;
			for (int i = 0; i < 36; ++i)
			{
				m_lineRenderer.SetPosition(i, m_circleCentre + point);
				point = rotation * point;
			}
		}
		
		m_lastPosition = m_target.position;
		
		m_firstHalf.Enqueue(m_start);
		m_secondHalf.Enqueue(m_middle);
	}
	
	public override Vector3 PredictPosition (float secondsInFuture)
	{
		if (m_isLinear)
		{
			return m_start + m_direction.normalized * m_speed * secondsInFuture;
		}
		Vector3 difference = m_start - m_circleCentre;
		
		float distanceToMove = secondsInFuture * m_speed;
		float angleInRadians = distanceToMove / difference.magnitude;

		Quaternion rotation = Quaternion.AngleAxis(-angleInRadians * Mathf.Rad2Deg, m_circleNormal);
		
		return m_circleCentre + rotation * difference;
	}
	
	private static int queueSize = 5;
	private Queue<Vector3> m_firstHalf;
	private Queue<Vector3> m_secondHalf;
	private Vector3 m_direction;
	private Vector3 m_lastPosition;
	private Vector3 m_start;
	private Vector3 m_middle;
	private Vector3 m_end;
	//private Vector3[] m_circlePositions = new Vector3[21];
	private Vector3 m_circleCentre;
	private Vector3 m_circleNormal;
	private bool m_isLinear;
	private float m_speed;
	private int m_validCount;
}






