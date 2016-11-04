using UnityEngine;
using System.Collections;

/*
Generates waves, just changes the mesh of the water.
Used script found on the unity forms here: http://answers.unity3d.com/questions/443031/sinus-for-rolling-waves.html
as it was better than what I worked on for an hour or two >_>
-Ryan Black

Modified this Script to then call setWaterLevel on the boat to the current water level
of the boat.
Diego Wilde
*/


public class WaveGen : MonoBehaviour {

    public float amplitude = 1f;
    public float frequency = 1.0f;
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;

    private Vector3[] baseHeight;
    private Vector3[] vertices;
    void Start()
    {

    }

	void FixedUpdate () {
        Buoyancy buoyancy = GetComponent<Buoyancy>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        if (baseHeight == null)
        {
            baseHeight = mesh.vertices;
        }
        vertices = new Vector3[baseHeight.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = baseHeight[i];
            vertex.y += Mathf.Sin(Time.time * frequency + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * amplitude;
            vertex.y += Mathf.PerlinNoise(baseHeight[i].x + noiseWalk, baseHeight[i].y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
            vertices[i] = vertex;
        }
        
        /* Another attempt
        for (int j = 0; j < vertices.Length; j++)
        {
            if((vertices[j].x == buoyancy.getPosition().x)
                && (vertices[j].z == buoyancy.getPosition().z))
                buoyancy.setWaterLevel(vertices[j].y);
        }
        */
        

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    public Vector3[] getWaves()
    {
        //Mesh mesh = GetComponent<MeshFilter>().mesh;
        return vertices;
    }
}
