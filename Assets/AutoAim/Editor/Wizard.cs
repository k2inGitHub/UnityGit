// --------------------------------------------------------------------------
//  Copyright © 2012 Timothy Aidley
//  Copyright © 2012 - 2014 Timothy Aidley
//  See http://www.thegamemechanics.co.uk/autoaim/
// --------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class Wizard : EditorWindow
{

	[MenuItem("Window/AutoAim Turret Wizard")]
	static void CreateWizard()
	{
		Wizard window = EditorWindow.GetWindow<Wizard>(true, "Set up a turret");
		window.ShowUtility();
		window.minSize = new Vector2(320, 280);
	}
	
	enum Step
	{
		Introduction,
		SelectBase,
		SelectBasicOrSwivel,
		SelectSwivel,
		SelectGunBarrel,
		SelectProjectileType,
		SelectTargetType,
		ChooseLimits,
		ChooseTargeter,
		ChooseBullets,
		Summary,
		
	}
	
	void AddTexture(string name)
	{
		m_textures.Add(name, (Texture)AssetDatabase.LoadAssetAtPath(m_resourcesDirectory + name + ".png", typeof(Texture)));
	}
	
	
	void OnEnable()
	{
		m_textures = new Dictionary<string, Texture>();
		
		m_resourcesDirectory = "Assets/AutoAim/Editor/Resources/";
		
		var possibleAutoaims = Directory.GetDirectories(Application.dataPath,"AutoAim", SearchOption.AllDirectories);
		foreach(var directory in possibleAutoaims)
		{
			var resourceDirs = Directory.GetDirectories(directory, "Resources", SearchOption.AllDirectories);
			
			if (resourceDirs.Length > 0)
			{
				m_resourcesDirectory = resourceDirs[0].Substring(Application.dataPath.Length - 6).Replace("\\", "/") + "/";
			}
		}
		
		AddTexture("basicturret");
		AddTexture("basicturret_highlight");
		AddTexture("basicturret_base");
		AddTexture("basicturret_barrel");
		AddTexture("swivelturret");
		AddTexture("swivelturret_highlight");
		AddTexture("swivelturret_swivel");
		AddTexture("swivelturret_barrel");
		AddTexture("splash");
		AddTexture("parabolic_bullet");
		AddTexture("parabolic_bullet_highlight");
		AddTexture("linear_bullet");
		AddTexture("linear_bullet_highlight");
		AddTexture("linear_target");
		AddTexture("linear_target_highlight");
		AddTexture("parabolic_target");
		AddTexture("parabolic_target_highlight");
		AddTexture("curved_target");
		AddTexture("curved_target_highlight");
		AddTexture("limits");
		AddTexture("target");
		AddTexture("prefabshooter");
		
		
		m_titleStyle = new GUIStyle();
		m_titleStyle.fontSize = 20;
		m_titleStyle.stretchWidth = true;
			
	}
	
	
	void OnGUI()
	{
		string functionName = m_step.ToString() + "GUI";
		System.Type wizardType = this.GetType();
		System.Reflection.MethodInfo methodInfo = wizardType.GetMethod(functionName);
		
		EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
		
		bool nextEnabled = true;
		if (methodInfo != null)
		{
			System.Object boolObj = methodInfo.Invoke(this, null);
			if (boolObj != null)
			{
				nextEnabled = (bool)boolObj;
			}
		}
		else
		{
			GUILayout.Label("No function called " + functionName);
		}
		
		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		
		GUI.enabled = (m_step != Step.Introduction);
		if (GUILayout.Button("< Back"))
		{
			if ( (m_step == Step.SelectGunBarrel) && !m_swivel)
			{
				m_step--;
			}
			m_step--;
		}
		GUILayout.FlexibleSpace();
		GUI.enabled = nextEnabled;
		if (m_step == Step.Summary)
		{
			if (GUILayout.Button("Apply Turret!"))
			{
				ApplyTurret();
			}
		}
		else
		{
			if (GUILayout.Button("Next >"))
			{
				if ( (m_step == Step.SelectBasicOrSwivel) && !m_swivel)
				{
					m_step++;
				}
				m_step++;
			}
		}
		
		EditorGUILayout.EndHorizontal();
	}
	
	public void Diagram(string diagram)
	{

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(m_textures[diagram]);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

	}
	
	
	public bool IntroductionGUI()
	{
		GUILayout.Label("Create a turret", m_titleStyle);
		GUILayout.Label("This Wizard will guide you through the process of creating a turret.", EditorStyles.wordWrappedLabel);
		Diagram("splash");
		
		return true;	
	}
	
	public bool SelectBaseGUI()
	{
		GUILayout.Label("Select the turret base", m_titleStyle);
		
		Diagram("basicturret_base");
		
		GUILayout.Space(10);
		
		m_baseObject = EditorGUILayout.ObjectField("Turret Base", m_baseObject, typeof(GameObject), true) as GameObject;
		GUILayout.Label("Drag the base object for your turret into the box above.", EditorStyles.wordWrappedLabel);
		
		if (!m_baseObject)
		{
			GUILayout.FlexibleSpace();
			EditorGUILayout.HelpBox("You must select a base object to continue.", MessageType.Warning);
			return false;
		}
		
		return true;
	}
	
	public bool SelectBasicOrSwivelGUI()
	{
		GUILayout.Label("Choose turret type", m_titleStyle);
		GUILayout.Label("Do you want a basic or swivel turret?");
		

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		m_swivel = !GUILayout.Toggle(!m_swivel, m_swivel ? m_textures["basicturret"] : m_textures["basicturret_highlight"], GUI.skin.label);
		m_swivel = GUILayout.Toggle(m_swivel, m_swivel ? m_textures["swivelturret_highlight"] : m_textures["swivelturret"], GUI.skin.label);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		if (m_swivel)
		{
			EditorGUILayout.HelpBox("A swivel on the base rotates horizontally while the gun itself rotates vertically.", MessageType.Info);
		}
		else
		{
			EditorGUILayout.HelpBox("The gun can rotate in all directions.", MessageType.Info);
		}
		
		return true;
	}
	
	public bool SelectSwivelGUI()
	{
		GUILayout.Label("Select the horizontal swivel", m_titleStyle);
		
		Diagram("swivelturret_swivel");

		m_swivelObject = EditorGUILayout.ObjectField("Gun Barrel", m_swivelObject, typeof(GameObject), true) as GameObject;
		GUILayout.Space(10);
		GUILayout.Label("Drag the swivel for your turret into the box above.", EditorStyles.wordWrappedLabel);
		
		if (!m_swivelObject)
		{
			GUILayout.FlexibleSpace();
			EditorGUILayout.HelpBox("You must select a horizontal swivel object to continue.", MessageType.Warning);
			return false;
		}
		
		return true;
	}
	
	
	public bool SelectGunBarrelGUI()
	{
		GUILayout.Label("Select the turret gun barrel", m_titleStyle);
		
		if (m_swivel)
		{
			Diagram("swivelturret_barrel");
		}
		else
		{
			Diagram("basicturret_barrel");
		}
		GUILayout.Space(10);
		
		m_gunBarrelObject = EditorGUILayout.ObjectField("Gun Barrel", m_gunBarrelObject, typeof(GameObject), true) as GameObject;
		GUILayout.Space(10);
		GUILayout.Label("Drag the gun barrel for your turret into the box above.", EditorStyles.wordWrappedLabel);
		
		if (!m_gunBarrelObject)
		{
			GUILayout.FlexibleSpace();
			EditorGUILayout.HelpBox("You must select a gun barrel object to continue.", MessageType.Warning);
			return false;
		}
		
		return true;
	}
	
	public bool SelectProjectileTypeGUI()
	{
		GUILayout.Label("Choose projectile type", m_titleStyle);
		GUILayout.Label("Are they affected by gravity?");
		

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		m_projectileHasGravity = !GUILayout.Toggle(!m_projectileHasGravity, m_projectileHasGravity ? m_textures["linear_bullet"] : m_textures["linear_bullet_highlight"], GUI.skin.label);
		m_projectileHasGravity = GUILayout.Toggle(m_projectileHasGravity, m_projectileHasGravity ? m_textures["parabolic_bullet_highlight"] : m_textures["parabolic_bullet"], GUI.skin.label);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		if (m_projectileHasGravity)
		{
			EditorGUILayout.HelpBox("The bullet is affected by gravity, and will travel in an arc.", MessageType.Info);
		}
		else
		{
			EditorGUILayout.HelpBox("The bullet is unaffected by gravity, and will travel in straight line.", MessageType.Info);
		}
		
		return true;
	}
	
	
	public bool SelectTargetTypeGUI()
	{
		GUILayout.Label("How do the targets move?", m_titleStyle);
		GUILayout.Space(10);
		
		bool isLinear = (m_targetType == Aimer.TargetPredictionType.Linear);
		bool isParabolic = (m_targetType == Aimer.TargetPredictionType.Parabolic);
		bool isCircular = (m_targetType == Aimer.TargetPredictionType.Circular);
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		bool newLinear = GUILayout.Button(isLinear ? m_textures["linear_target_highlight"] : m_textures["linear_target"], GUI.skin.label);
		bool newParabolic = GUILayout.Button(isParabolic ? m_textures["parabolic_target_highlight"] : m_textures["parabolic_target"], GUI.skin.label);
		bool newCircular = GUILayout.Button(isCircular ? m_textures["curved_target_highlight"] : m_textures["curved_target"], GUI.skin.label);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		if (Event.current.type == EventType.Layout)
		{
			// do nothing
		}
		else if (newLinear && (isLinear != newLinear))
		{
			m_targetType = Aimer.TargetPredictionType.Linear;
		}
		else if (newParabolic && (isParabolic != newParabolic))
		{
			m_targetType = Aimer.TargetPredictionType.Parabolic;
		}
		else if (newCircular && (isCircular != newCircular))
		{
			m_targetType = Aimer.TargetPredictionType.Circular;
		}
		
		string[] guides = {
			"Use this for targets that mainly travel in a straight line.\nThis is uses the least CPU power.",
			"Use this for targets that have been 'flung' under the influence of gravity.",
			"Use this for the general case where the target moves with some kind of smooth motion.\nThis uses the most CPU power."
		};
		EditorGUILayout.HelpBox(guides[(int)m_targetType], MessageType.Info);
		
		GUILayout.Space(10);
		GUILayout.Label("If in doubt, use 'Linear'.\nIt uses the least CPU and does a good job in most situations.", EditorStyles.wordWrappedLabel);
		
		return true;
	}
	
	public bool ChooseLimitsGUI()
	{
		GUILayout.Label("Limit the arc of fire?", m_titleStyle);
		GUILayout.Space(10);
		Diagram("limits");
		GUILayout.Space(10);
		m_hasLimits = EditorGUILayout.Toggle("Turret has limits?", m_hasLimits);
		GUILayout.Space(10);
		GUILayout.Label("Left alone, the turret will be able to point in any direction\n." +
						"If this checkbox is ticked, a Limiter will be added so that you can limit the angle, range and rotation speed of the turret.", EditorStyles.wordWrappedLabel);
		
		return true;
	}
	
	public bool ChooseTargeterGUI()
	{
		GUILayout.Label("Use automatic targeter?", m_titleStyle);
		Diagram("target");
		m_hasTargeter = EditorGUILayout.Toggle("Automatic targeting", m_hasTargeter);
		GUILayout.Space(10);
		if (m_hasTargeter)
		{
			GUILayout.Label("The targeter used object tags to choose its target.\n" +
							"What tag would you like to use for your target?.", EditorStyles.wordWrappedLabel);
			
			m_targetTag = EditorGUILayout.TagField("Tag to target", m_targetTag);
			
			if (m_targetTag == "")
			{
				EditorGUILayout.HelpBox("You must select a target tag.", MessageType.Warning);
				return false;
			}
			
		}
		
		return true;
	}
	
	public bool ChooseBulletsGUI()
	{
		GUILayout.Label("Use the automatic shooter?", m_titleStyle);
		GUILayout.Space(5);
		Diagram("prefabshooter");
		GUILayout.Space(5);
		m_hasShooter = EditorGUILayout.Toggle("Use bullet shooter", m_hasShooter);
		GUILayout.Space(5);
		if (m_hasShooter)
		{
			m_bulletPrefab = EditorGUILayout.ObjectField("Bullet prefab", m_bulletPrefab, typeof(GameObject), false) as GameObject;
			GUILayout.Space(10);
			GUILayout.Label("Drag the bullet prefab into the box above.", EditorStyles.wordWrappedLabel);
			
			if (!m_bulletPrefab)
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.HelpBox("You must select a base object to continue.", MessageType.Warning);
				return false;
			}
			else if (PrefabUtility.GetPrefabType(m_bulletPrefab) != PrefabType.Prefab)
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.HelpBox("Selected object must be a prefab.", MessageType.Warning);
				return false;
			}
		}
		
		return true;
	}
	
	public bool SummaryGUI()
	{
		GUILayout.Label("Summary", m_titleStyle);
		GUILayout.Space(10);
		EditorGUILayout.LabelField("Base object:", m_baseObject.name);
		EditorGUILayout.LabelField("Gun Barrel object:", m_gunBarrelObject.name);
		EditorGUILayout.LabelField("Swivel object:", m_swivelObject ? m_swivelObject.name : "NO");
		EditorGUILayout.LabelField("Bullet has gravity:", m_projectileHasGravity ? "YES" : "NO");
		EditorGUILayout.LabelField("Turret has limits:", m_hasLimits ? "YES" : "NO");
		EditorGUILayout.LabelField("Turret has targeter:", m_hasTargeter ? "YES" : "NO");
		if (m_hasTargeter)
		{
			EditorGUILayout.LabelField("Target tag:", m_targetTag);
		}
		EditorGUILayout.LabelField("Turret has shooter:", m_hasShooter ? "YES" : "NO");
		if (m_hasShooter)
		{
			EditorGUILayout.LabelField("Shooter prefab:", m_bulletPrefab.name);
		}
		
		return true;
	}
	
	public void ApplyTurret()
	{
		Aimer aimer = m_baseObject.AddComponent<Aimer>();
		aimer.m_gunObject = m_gunBarrelObject;
		if (m_swivel)
		{
			aimer.m_optionalSwivel = m_swivelObject;
		}
		if (m_projectileHasGravity)
		{
			aimer.m_projectileType = Aimer.ProjectilePredictionType.Parabolic;
		}
		else
		{
			aimer.m_projectileType = Aimer.ProjectilePredictionType.Linear;
		}
		aimer.m_targetType = m_targetType;
		if (m_hasLimits)
		{
			m_baseObject.AddComponent<Limiter>();
			aimer.m_applyAimToBarrel = false;
		}
		else
		{
			aimer.m_applyAimToBarrel = true;
		}
		if (m_hasTargeter)
		{
			Targeter targeter = m_baseObject.AddComponent<Targeter>();
			targeter.m_targetTag = m_targetTag;
		}
		if (m_hasShooter)
		{
			PrefabShooter shooter = m_baseObject.AddComponent<PrefabShooter>();
			shooter.m_bulletPrefab = m_bulletPrefab;
		}
		
		Close();
		
		Selection.activeGameObject = m_baseObject;
		
		EditorUtility.DisplayDialog("Turret settings applied",
									"The settings have been applied to your turret!\n" +
									"Don't forget all settings in the inspector have help that can be " +
									"viewed by clicking on the little '?' to their right.",
									"OK");
		
	}
	
	private GUIStyle m_titleStyle;
	
	private Step m_step = Step.Introduction;
	private bool m_swivel = false;
	private GameObject m_baseObject = null;
	private GameObject m_gunBarrelObject = null;
	private GameObject m_swivelObject = null;
	private GameObject m_bulletPrefab = null;
	private Aimer.TargetPredictionType m_targetType = Aimer.TargetPredictionType.Linear;
	private bool m_projectileHasGravity = false;
	private bool m_hasLimits = true;
	private bool m_hasTargeter = true;
	private bool m_hasShooter = true;
	private string m_targetTag = "";
	
	private string m_resourcesDirectory = "Assets/AutoAim/Editor/Resources/";
	
	private Dictionary<string, Texture> m_textures;
}

