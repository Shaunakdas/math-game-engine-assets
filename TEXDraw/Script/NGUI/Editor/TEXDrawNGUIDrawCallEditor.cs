using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TEXDrawNGUIDrawCall))]
[CanEditMultipleObjects]
public class TEXDrawNGUIDrawCallEditor : Editor {
	
	SerializedProperty material;
	SerializedProperty index;
	//SerializedProperty drawCall;
	void OnEnable()
	{
		material = serializedObject.FindProperty("material");
		index = serializedObject.FindProperty("index");
		//drawCall = serializedObject.FindProperty("drawCall");
	}
	 
	public override void OnInspectorGUI()
	{
		
		if(index.hasMultipleDifferentValues)
			EditorGUILayout.LabelField("Index", "(Variant)");
		else
		{
			var en = ((MonoBehaviour)target).enabled;
			var mat = material.objectReferenceValue;
			EditorGUILayout.LabelField("Index", string.Format("{0} ({1}), ({2})", 
				index.intValue, en ? "Active" : "Inactive", mat ? mat.name : ""));
		}
		/*
		var r = EditorGUILayout.GetControlRect();
		r.width /=2;
		EditorGUI.ObjectField(r, material, GUIContent.none);
		r.x += r.width;
		EditorGUI.ObjectField(r, drawCall, GUIContent.none);
		*/
	}
}
