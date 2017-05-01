using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TEXDrawNGUIPanelManager))]
[CanEditMultipleObjects]
public class TEXDrawNGUIPanelManagerEditor : Editor {
	SerializedProperty material;
	
	static GUIContent matLabel = new GUIContent("Material");
	//SerializedProperty drawCall;
	void OnEnable()
	{
		material = serializedObject.FindProperty("panelMaterial");
	}
	
	public override void OnInspectorGUI()
	{
		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.PropertyField(material, matLabel);
		EditorGUI.EndDisabledGroup();
		if(material.hasMultipleDifferentValues)
			return;
		var obj = (TEXDrawNGUIPanelManager)target;
		var r = EditorGUILayout.GetControlRect();
		var b = r.width;
		r.width = Mathf.Min(r.width, EditorGUIUtility.labelWidth + 30);
		EditorGUI.LabelField(r, "Draw Calls Count", obj.texDrawCalls.size.ToString());
		//Buttons....
		if(b - r.width < 0)
			return;
		EditorGUI.BeginDisabledGroup(!(obj.enabled && obj.CanCleanup()));
		r.x += r.width;
		r.width = b - r.width;
		if(GUI.Button(r, "Cleanup"))
			obj.Cleanup();
		EditorGUI.EndDisabledGroup();
	}
}
