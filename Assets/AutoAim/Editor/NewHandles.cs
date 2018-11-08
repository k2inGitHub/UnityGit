// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;


public class NewHandles
{

	public static float AngleControl( Vector3 centre, Vector3 normal, Vector3 angleOrigin, float angle, float radius, Handles.DrawCapFunction capFunction, bool capLeft )
	{
		Vector3 right = Vector3.Cross(angleOrigin, normal);
		int controlId = GUIUtility.GetControlID(FocusType.Passive);
		
		Vector3 startPos = Quaternion.AngleAxis(angle, normal) * (angleOrigin * radius) + centre;
		
		float controlSize = HandleUtility.GetHandleSize(startPos) / 5.0f;
		
		float capAngle = capLeft ? -90.0f : 90.0f;
		
		float newAngle = angle;
		
		switch(Event.current.GetTypeForControl(controlId))
		{
		case EventType.Layout:
			HandleUtility.AddControl( controlId, HandleUtility.DistanceToCircle( startPos, controlSize ) );
			break;
		case EventType.Repaint:
			Color oldCol = Handles.color;
			if ( GUIUtility.hotControl == controlId )
			{
				Handles.color = Color.yellow;
			}	
			capFunction( controlId, startPos, Quaternion.AngleAxis(angle + capAngle, normal), controlSize );
			Handles.color = oldCol;
			break;
		case EventType.MouseDown:
			if ((HandleUtility.nearestControl == controlId) && (Event.current.button == 0))
			{
				GUIUtility.hotControl = controlId;
				Event.current.Use();
			}
			break;
		case EventType.MouseUp:
			if (GUIUtility.hotControl == controlId)
			{
				GUIUtility.hotControl = 0;
				Event.current.Use();
			}
			break;
		case EventType.MouseDrag:
			if (GUIUtility.hotControl == controlId)
			{
				Plane plane = new Plane(normal, centre);
				float distance;
				Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				ray.origin = Handles.matrix.inverse.MultiplyPoint3x4(ray.origin);
				ray.direction = Handles.matrix.inverse.MultiplyVector(ray.direction);
				plane.Raycast(ray, out distance); 
				Vector3 closestPoint = ray.GetPoint(distance);
				Vector3 direction = closestPoint - centre;
				//Vector3 closestPoint = HandleUtility.ClosestPointToDisc( centre, normal, radius );
				newAngle = Mathf.Rad2Deg * Mathf.Atan2( Vector3.Dot( -right, direction ), Vector3.Dot( angleOrigin, direction ) );
				
				GUI.changed = true;
				Event.current.Use();
			}
			break;
			
			
		}
		
		return newAngle;
	}
	
	private static float s_arrowScale = 1.0f / 3.0f;
	
	private static Vector3[] s_arrowPoints =
	{
		new Vector3( 1, 0, 1 ) * s_arrowScale,
		new Vector3( 2, 0, 1 ) * s_arrowScale,
		new Vector3( 0, 0, 3 ) * s_arrowScale,
		new Vector3( -2, 0, 1 ) * s_arrowScale,
		new Vector3( -1, 0, 1 ) * s_arrowScale,
		new Vector3( -1, 0, -1 ) * s_arrowScale,
		new Vector3( -2, 0, -1 ) * s_arrowScale,
		new Vector3( 0, 0, -3 ) * s_arrowScale,
		new Vector3( 2, 0, -1 ) * s_arrowScale,
		new Vector3( 1, 0, -1 ) * s_arrowScale,
		new Vector3( 1, 0, 1 ) * s_arrowScale,
	};
	
	public static void DoubleArrowCap(int controlId, Vector3 position, Quaternion rotation, float size)
	{
		Vector3[] points = new Vector3[11];
		System.Array.Copy(s_arrowPoints, points, 11);
		
		for (int i = 0; i < 11; ++i)
		{
			points[i] = (rotation * (points[i] * size)) + position;
		}
		
		Handles.DrawPolyLine(points);
	}	
}

