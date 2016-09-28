using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(NormalEditor))]
public class NormalEditorEditor : Editor
{
	NormalEditor m_norm;
	void OnEnable()
	{
		m_norm = target as NormalEditor;
		Undo.undoRedoPerformed += ApplyNewNormals;
	}

	void OnSceneGUI()
	{
		if (m_norm == null || m_norm.Mesh == null)
			return;

		for (int i = 0; i < m_norm.Mesh.vertexCount; i++)
		{
			Handles.color = Color.blue;
			Handles.matrix = m_norm.transform.localToWorldMatrix;
			Handles.Label(m_norm.Mesh.vertices[i], i.ToString());

			Handles.color = Color.yellow;
			Handles.DrawLine(
				m_norm.Mesh.vertices[i],
				m_norm.Mesh.vertices[i] + m_norm.Mesh.normals[i]);
		}
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if (EditorGUI.EndChangeCheck())
		{
			ApplyNewNormals();
		}
	}

	void ApplyNewNormals()
	{
		if (!Application.isPlaying)
		{
			m_norm.ApplyNewNormals();
		}
	}

	void OnDisable()
	{
		Undo.undoRedoPerformed -= ApplyNewNormals;
	}
}