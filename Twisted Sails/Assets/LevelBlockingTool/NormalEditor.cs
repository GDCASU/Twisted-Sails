using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NormalEditor : MonoBehaviour
{
	Mesh m_mesh;
	public Mesh Mesh
	{
		get
		{
			return m_mesh;
		}
	}

	[SerializeField]
	Vector3[] m_normals;

	void OnEnable()
	{
		m_mesh = GetComponent<MeshFilter>().sharedMesh;
		m_normals = m_mesh.normals;
	}

	public void ApplyNewNormals()
	{
		Vector3[] fixedNormals = new Vector3[m_normals.Length];
		for (int i = 0; i < m_normals.Length; i++)
		{
			fixedNormals[i] = m_normals[i];
			fixedNormals[i].Normalize();
		}
		m_mesh.normals = fixedNormals;
	}
}