using UnityEngine;
using System.Collections;
using UnityEditor;
using TexDrawLib;

[CustomEditor(typeof(TEXDrawNGUI))]
public class TEXDrawNGUIEditor : UIWidgetInspector
{
	SerializedProperty m_Text;
	SerializedProperty m_FontIndex;
	SerializedProperty m_AutoFit;
	SerializedProperty m_Size;
	SerializedProperty m_SpaceSize;
	SerializedProperty m_AutoWrap;
	//SerializedProperty m_Color;
	SerializedProperty m_Material;
    SerializedProperty m_Filling;

	SerializedProperty m_debugReport;
	//static bool foldExpand = false;

	// Use this for initialization
	protected override void OnEnable()
	{
		base.OnEnable();

		m_Text = serializedObject.FindProperty("m_Text");
		m_FontIndex = serializedObject.FindProperty("m_FontIndex");
		m_Size = serializedObject.FindProperty("m_Size");
		m_AutoFit = serializedObject.FindProperty("m_AutoFit");
		m_AutoWrap = serializedObject.FindProperty("m_AutoWrap");
		m_SpaceSize = serializedObject.FindProperty("m_SpaceSize");
		//m_Color = serializedObject.FindProperty("m_Color");
		m_Material = serializedObject.FindProperty("m_Material");
        m_Filling = serializedObject.FindProperty("m_AutoFill");
		m_debugReport = serializedObject.FindProperty("debugReport");		
		Undo.undoRedoPerformed += Redraw;
	}
	
	
	protected override void OnDisable()
	{
		Undo.undoRedoPerformed -= Redraw;
	}

	#if LEGACY_NGUI
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		base.OnInspectorGUI ();
		serializedObject.ApplyModifiedProperties();
	}
	#endif

	#if LEGACY_NGUI 
	protected override bool DrawProperties ()
	#else
	protected override void DrawCustomProperties ()
	#endif
	{
		EditorGUIUtility.labelWidth += 30;
		EditorGUI.BeginChangeCheck();

		TEXBoxHighlighting.DrawText(m_Text);
		
		if (serializedObject.targetObjects.Length == 1)
		{
			if (m_debugReport.stringValue != System.String.Empty)
				EditorGUILayout.HelpBox(m_debugReport.stringValue, MessageType.Warning);
		}

		EditorGUILayout.PropertyField(m_Size);
		//foldExpand = EditorGUILayout.Foldout(foldExpand, "More Properties");
		//if (foldExpand)
		{
			EditorGUI.indentLevel++;
			TEXSharedEditor.DoFontIndexSelection(m_FontIndex);

			EditorGUILayout.PropertyField(m_AutoFit);
            EditorGUI.BeginDisabledGroup(m_AutoFit.enumValueIndex == 2);
			EditorGUILayout.PropertyField(m_AutoWrap);
            EditorGUI.EndDisabledGroup();
			EditorGUILayout.PropertyField(m_SpaceSize);
			TEXSharedEditor.DoMaterialGUI(m_Material, (ITEXDraw)target);
			EditorGUILayout.PropertyField(m_Filling);      
            //EditorGUILayout.IntField(((TEXDrawNGUI)target).managerQ);      
    		EditorGUI.indentLevel--;
		}


		if (EditorGUI.EndChangeCheck())
			Redraw();

		EditorGUIUtility.labelWidth -= 30;

		#if LEGACY_NGUI
		return true;
        #else
        base.DrawCustomProperties();
		#endif
	}

	public void Redraw()
	{ 
		foreach (TEXDrawNGUI i in (serializedObject.targetObjects))
		{
			i.SetTextDirty();
			i.MarkAsChanged();
		}
	}

    [MenuItem("NGUI/Create/TEXDraw")]
    static public void AddTexDraw ()
    {
        GameObject go = NGUIEditorTools.SelectedRoot(true);

        if (go != null)
        {
            TEXDrawNGUI tex = NGUITools.AddWidget<TEXDrawNGUI>(go);
            tex.name = "TexDraw";
            tex.pivot = NGUISettings.pivot;
            tex.width = 100;
            tex.height = 100;
            Selection.activeGameObject = tex.gameObject;
        }
        else
        {
            Debug.Log("You must select a game object first.");
        }
    }
   
}

