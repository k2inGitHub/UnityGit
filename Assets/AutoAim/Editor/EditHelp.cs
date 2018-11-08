// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;




public class EditHelp
{
	// This hash set stores which help sections are expanded.
	private HashSet<int> m_showHelp = new HashSet<int>();
	// This index is incremented with each control, and indexes the hash set.
	private int m_index;
	private SerializedObject m_object;
	
	private List< KeyValuePair<string, PropertyInterface>> m_properties = new List<KeyValuePair<string, PropertyInterface>>();
	
	public class PropertyInterface
	{
		protected string m_label;
		protected string m_helpText;
		protected bool m_enabled;
		
		public PropertyInterface(string label, string helpText)
		{
			m_label = label;
			m_helpText = helpText;
			m_enabled = true;
		}
		
		
		public virtual void EditProperty(EditHelp helper, SerializedProperty property)
		{		
			helper.StartHelp( m_helpText );
			
			EditorGUILayout.PropertyField(property, new GUIContent( m_label ) );
			
			helper.EndHelp();
		}
		
		public bool Enabled
		{
			get { return m_enabled; }
			set { m_enabled = value; }
		}
		
		public string HelpText
		{
			get { return m_helpText; }
			set { m_helpText = value; }
		}
		
	}
	
	public class EnumPropertyInterface : PropertyInterface
	{
		protected string[] m_enumHelpLines;
		
		public EnumPropertyInterface(string label, string helpText, params string[] enumHelpLines) : base(label, helpText)
		{
			m_enumHelpLines = enumHelpLines;
		}
		
		public override void EditProperty(EditHelp helper, SerializedProperty property)
		{
			string contextHelp;
			try
			{
				contextHelp = m_enumHelpLines[property.enumValueIndex];
			}
			catch (System.IndexOutOfRangeException)
			{
				contextHelp = "ERROR: Due to version incompatibilities, you need to re-set this value";
			}
			helper.StartHelp( m_helpText + "\n\n" + contextHelp );
			EditorGUILayout.PropertyField(property, new GUIContent( m_label ) );	
			helper.EndHelp();
		}
	}
	
	public class BoolPropertyInterface : PropertyInterface
	{
		protected string m_trueHelp;
		protected string m_falseHelp;
		
		public string TrueHelp
		{
			get { return m_trueHelp; }
			set { m_trueHelp = value; }
		}
		
		public string FalseHelp
		{
			get { return m_falseHelp; }
			set { m_falseHelp = value; }
		}
		
		
		public BoolPropertyInterface(string label, string helpText, string trueHelp, string falseHelp) : base(label, helpText)
		{
			m_trueHelp = trueHelp;
			m_falseHelp = falseHelp;
		}
		
		public override void EditProperty(EditHelp helper, SerializedProperty property)
		{		
			helper.StartHelp( m_helpText + "\n\n" + (property.boolValue ? m_trueHelp : m_falseHelp) );
			EditorGUILayout.PropertyField(property, new GUIContent( m_label ) );	
			helper.EndHelp();
		}
	}
	
	public class SliderPropertyInterface : PropertyInterface
	{
		protected float m_minimum;
		protected float m_maximum;
		
		public SliderPropertyInterface(string label, string helpText, float minimum, float maximum) : base(label, helpText)
		{
			m_minimum = minimum;
			m_maximum = maximum;
		}
		
		public override void EditProperty(EditHelp helper, SerializedProperty property)
		{		
			helper.StartHelp( m_helpText  );
			
			EditorGUILayout.Slider(property, m_minimum, m_maximum, m_label);	
			helper.EndHelp();
		}
	}
	
	public class TagPropertyInterface : PropertyInterface
	{
		public TagPropertyInterface(string label, string helpText) : base(label, helpText)
		{

		}
				
		public override void EditProperty(EditHelp helper, SerializedProperty property)
		{		
			helper.StartHelp( m_helpText  );
			property.stringValue = EditorGUILayout.TagField(m_label, property.stringValue);
			helper.EndHelp();
		}
	}
	
	public class ArrayPropertyInterface : PropertyInterface
	{
		bool foldoutVisible = false;
		string m_itemName;
				
		public ArrayPropertyInterface(string label, string helpText, string itemName) : base(label, helpText)
		{
			m_itemName = itemName;
		}
		
		public override void EditProperty(EditHelp helper, SerializedProperty property)
		{
			helper.StartHelp( m_helpText );
			EditorGUILayout.BeginVertical();
			foldoutVisible = EditorGUILayout.Foldout(foldoutVisible, new GUIContent(m_label));
			if (foldoutVisible)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10);
				EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
				for (int i = 0; i < property.arraySize; ++i)
				{
					SerializedProperty childProperty = property.GetArrayElementAtIndex(i);
					EditorGUILayout.BeginHorizontal(GUI.skin.box);
					string propertyName = string.Format("{0} {1}", m_itemName, i);
					EditorGUILayout.PropertyField(childProperty, new GUIContent(propertyName), true);;
					
					if (GUILayout.Button("Del", GUILayout.ExpandWidth(false)))
					{
						property.DeleteArrayElementAtIndex(i);
					}
					
					
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
				{
					property.arraySize = property.arraySize + 1;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				
				
			}
			EditorGUILayout.EndVertical();
			helper.EndHelp();
		}
	}

	
	public void AddPropertyInterface(string propertyName, PropertyInterface propInterface)
	{
		m_properties.Add(new KeyValuePair<string, PropertyInterface>(propertyName, propInterface));
	}
	
	public void EditProperties( UnityEngine.Object obj )
	{
		m_object = new SerializedObject( obj );
		
		m_index = 0;
		
		foreach(var entry in m_properties)
		{
			GUI.enabled = entry.Value.Enabled;
			SerializedProperty property = m_object.FindProperty(entry.Key);
			if (property == null) continue;
			
			entry.Value.EditProperty(this, property);	

		}
		
		GUI.enabled = true;
		
		m_object.ApplyModifiedProperties();
	}
	
	
	// Should be called at the beginning of the OnInspectorGUI() function.
	public void BeginEditHelp( Object obj)
	{
		m_object = new SerializedObject(obj);
		m_index = 0;
	}
	
	public void EndEditHelp()
	{
		
	}
	
	public void DisplayHelp( string help )
	{
		if ( m_showHelp.Contains(m_index++) )
		{
			EditorGUILayout.HelpBox( help, MessageType.Info );
		}
		
	}
	
	
	
	
	// Start / End functions wrap a control to have help around it
	public void StartHelp(string help)
	{
		bool enabled = m_showHelp.Contains(m_index);
		bool guiEnabled = GUI.enabled;
		GUI.enabled = true;
		
		if ( enabled )
		{
			
			EditorGUILayout.HelpBox( help, MessageType.Info );

			Rect btnRect = GUILayoutUtility.GetLastRect();
			btnRect.x = btnRect.x + btnRect.width - 16;
			btnRect.y = btnRect.y + 0;
			btnRect.width = 16;
			btnRect.height = 16;
			
			enabled ^= GUI.Button(btnRect, "X", EditorStyles.miniLabel);
				
			if (enabled)
			{
				m_showHelp.Add(m_index);
			}
			else
			{
				m_showHelp.Remove(m_index);
			}

		}
		EditorGUILayout.BeginHorizontal();
		GUI.enabled = guiEnabled;
	}
	
	// See above.
	public void EndHelp()
	{
		bool guiEnabled = GUI.enabled;
		GUI.enabled = true;
		bool enabled = m_showHelp.Contains(m_index);
		
		if (!enabled)
		{
			enabled ^= GUILayout.Button( "?", EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16));
			
			if (enabled)
			{
				m_showHelp.Add(m_index);
			}
			else
			{
				m_showHelp.Remove(m_index);
			}
		}
		else
		{
			GUILayout.Button( " ", EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16));
		}
		
		m_index++;
		
		EditorGUILayout.EndHorizontal();
		GUI.enabled = guiEnabled;
	}
	

}


