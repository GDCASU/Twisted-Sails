using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using System;

//CuboidMaker creates a new Unity Editor window called a Cuboid Maker
//The window shows a GUI that allows you to create simple cube-ish objects
//Author: Kyle Chapman
//First draft date: Wednesday, September 28

public class CuboidMaker : EditorWindow
{
	//the object bases four corners
	private Vector3 farLeftCorner;
	private Vector3 farRightCorner;
	private Vector3 nearRightCorner;
	private Vector3 nearLeftCorner;

	//the vector that is used to calculate the top four corners
	//the y component is the objects height
	//the x and z components dictate how much the objects skews forwards/backwards and left/right from the base
	private Vector3 heightAndSkew;

	//the material to assign to the cuboid when its created
	private Material materialToAssign;

	//whether the mesh should be stored in the assets system or not
	private bool storeMesh;

	//the string path representing the folder to put the mesh assets into
	private string pathToCreateAt;

	//the meshes name in the asset system
	private string meshName;

	//alows the creation of the unity editor window Cuboid Maker
	[MenuItem("Window/Cuboid Maker")]
	private static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(CuboidMaker));
	}

	//dictates the UI of the window, with labels, text areas and buttons
	private void OnGUI()
	{
		Rect farLeftRect = EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Far Left Corner: ");
		float fl1 = EditorGUILayout.FloatField(farLeftCorner.x);
		float fl2 = EditorGUILayout.FloatField(farLeftCorner.y);
		float fl3 = EditorGUILayout.FloatField(farLeftCorner.z);
		farLeftCorner = new Vector3(fl1, fl2, fl3);
		EditorGUILayout.EndHorizontal();

		Rect farRightRect = EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Far Right Corner: ");
		float fr1 = EditorGUILayout.FloatField(farRightCorner.x);
		float fr2 = EditorGUILayout.FloatField(farRightCorner.y);
		float fr3 = EditorGUILayout.FloatField(farRightCorner.z);
		farRightCorner = new Vector3(fr1, fr2, fr3);
		EditorGUILayout.EndHorizontal();

		Rect nearRightRect = EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Near Right Corner: ");
		float nr1 = EditorGUILayout.FloatField(nearRightCorner.x);
		float nr2 = EditorGUILayout.FloatField(nearRightCorner.y);
		float nr3 = EditorGUILayout.FloatField(nearRightCorner.z);
		nearRightCorner = new Vector3(nr1, nr2, nr3);
		EditorGUILayout.EndHorizontal();

		Rect nearLeftRect = EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Near Left Corner: ");
		float nl1 = EditorGUILayout.FloatField(nearLeftCorner.x);
		float nl2 = EditorGUILayout.FloatField(nearLeftCorner.y);
		float nl3 = EditorGUILayout.FloatField(nearLeftCorner.z);
		nearLeftCorner = new Vector3(nl1, nl2, nl3);
		EditorGUILayout.EndHorizontal();

		Rect heightSkewRect = EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Height and Skew: ");
		float hs1 = EditorGUILayout.FloatField(heightAndSkew.x);
		float hs2 = EditorGUILayout.FloatField(heightAndSkew.y);
		float hs3 = EditorGUILayout.FloatField(heightAndSkew.z);
		heightAndSkew = new Vector3(hs1, hs2, hs3);
		EditorGUILayout.EndHorizontal();

		Rect materialRect = EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Material to assign: ");
		materialToAssign = (Material)EditorGUILayout.ObjectField(materialToAssign, typeof(Material), true);
		EditorGUILayout.EndHorizontal();

		Rect storeMeshToggleRect = EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Store Mesh In Assets? ");
		storeMesh = EditorGUILayout.Toggle(storeMesh);
		EditorGUILayout.EndHorizontal();
		if (storeMesh)
		{
			Rect pathRect = EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Path To Store Mesh Asset: ");
			pathToCreateAt = EditorGUILayout.TextField(pathToCreateAt);
			EditorGUILayout.EndHorizontal();

			Rect nameRect = EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Mesh Name: ");
			name = EditorGUILayout.TextField(name);
			EditorGUILayout.EndHorizontal();
		}

		if (GUILayout.Button("Create Cuboid"))
		{
			CreateCuboid();
        }
	}

	private void CreateCuboid()
	{
		//generate the desired mesh
		Mesh generatedMesh = new Mesh();
		Vector3[] verticesList = new Vector3[] {
			farLeftCorner, //0
			farRightCorner, //1
			nearRightCorner, //2
			nearLeftCorner, //3
			farLeftCorner + heightAndSkew, //4
			farRightCorner + heightAndSkew, //5
			nearRightCorner + heightAndSkew, //6
			nearLeftCorner + heightAndSkew}; //7

		Vector3[] newVertices = new Vector3[0];
		newVertices = ConcatArrays(newVertices, verticesList);
		newVertices = ConcatArrays(newVertices, verticesList);
		newVertices = ConcatArrays(newVertices, verticesList);

		List<int[]> trianglesList = new List<int[]>();

		//bottom side triangles
		trianglesList.Add(new int[] { 0, 2, 1 });
		trianglesList.Add(new int[] { 0, 3, 2 });

		//top side triangles
		trianglesList.Add(new int[] { 4, 5, 7 });
		trianglesList.Add(new int[] { 5, 6, 7 });

		//right side triangles
		trianglesList.Add(new int[] { 10, 14, 13 });
		trianglesList.Add(new int[] { 9, 10, 13 });

		//left side triangles
		trianglesList.Add(new int[] { 8, 12, 15 });
		trianglesList.Add(new int[] { 8, 15, 11 });

		//back side triangles
		trianglesList.Add(new int[] { 17, 21, 20 });
		trianglesList.Add(new int[] { 16, 17, 20 });

		//front side triangles
		trianglesList.Add(new int[] { 19, 23, 22 });
		trianglesList.Add(new int[] { 18, 19, 22 });

		//concatenate the triangles into a list
		int[] newTriangles = new int[0];
		foreach (int[] triangle in trianglesList)
		{
			newTriangles = ConcatArrays(newTriangles, triangle);
		}

		//calculate the vertex normals
		Vector3[] newNormals = new Vector3[newVertices.Length];
		for (int i = 0; i < newNormals.Length; i++)
		{
			newNormals[i] = Vector3.zero;
		}
		
		//for each triangle, add its surface normal to the normal of the vertices that make up that triangle
		for (int i = 0; i < trianglesList.Count; i++)
		{
			int[] triangle = trianglesList[i];
			Vector3 vertex1 = newVertices[triangle[0]];
			Vector3 vertex2 = newVertices[triangle[1]];
			Vector3 vertex3 = newVertices[triangle[2]];
			Vector3 sideOne = (vertex2 - vertex1).normalized;
			Vector3 sideTwo = (vertex3 - vertex1).normalized;
			Vector3 surfaceNormal = Vector3.Cross(sideOne, sideTwo).normalized;
			newNormals[triangle[0]] += surfaceNormal;
			newNormals[triangle[1]] += surfaceNormal;
			newNormals[triangle[2]] += surfaceNormal;
		}

		//normalize all vertex normals
		for (int i = 0; i < newNormals.Length; i++)
		{
			newNormals[i] = newNormals[i].normalized;
			Debug.Log(newNormals[i]);
        }

		//asign the meshes properties
		generatedMesh.vertices = newVertices;
		generatedMesh.triangles = newTriangles;
		generatedMesh.normals = newNormals;
		//generatedMesh.RecalculateNormals(0);

		generatedMesh.name = name;

		//create the cuboid object and assign the mesh to it
		GameObject objectWithMesh = new GameObject();
		objectWithMesh.name = "Generated Cuboid";
		MeshFilter objectsMesh = objectWithMesh.AddComponent<MeshFilter>();
		objectsMesh.mesh = generatedMesh;
		MeshRenderer objectsRenderer = objectWithMesh.AddComponent<MeshRenderer>();
		objectsRenderer.material = materialToAssign;

		//store the mesh in the assets if the user wants it to
		if (storeMesh)
		{
			AssetDatabase.CreateAsset(generatedMesh, "Assets/" + pathToCreateAt + "/" + name + ".asset");
		}
    }

	//taken from stack overflow, concatenates two arrays together
	public static T[] ConcatArrays<T>(params T[][] list)
	{
		var result = new T[list.Sum(a => a.Length)];
		int offset = 0;
		for (int x = 0; x < list.Length; x++)
		{
			list[x].CopyTo(result, offset);
			offset += list[x].Length;
		}
		return result;
	}


}
