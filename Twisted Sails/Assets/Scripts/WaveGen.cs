using UnityEngine;
using System.Collections;

/*
Generates waves, just changes the mesh of the water.
Used script found on the unity forms here: http://answers.unity3d.com/questions/443031/sinus-for-rolling-waves.html
as it was better than what I worked on for an hour or two >_>

-Ryan Black
*/

public class WaveGen : MonoBehaviour {

    public float size = 0.1f;
    public float speed = 1.0f;
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;

    private Vector3[] baseHeight;

	void Update () {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        if (baseHeight == null)
        {
            baseHeight = mesh.vertices;
        }

        Vector3[] vertices = new Vector3[baseHeight.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = baseHeight[i];
            vertex.y += Mathf.Sin(Time.time * speed + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * size;
            vertex.y += Mathf.PerlinNoise(baseHeight[i].x + noiseWalk, baseHeight[i].y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
            vertices[i] = vertex;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
	}
}
