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
    public bool enableWaves = true;
    public float initialWaterLevel = 0;
    public float amplitude = 0.1f;
    public float frequency = 1.0f;
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;


    private Vector3[] baseHeight;
   // private Vector3[] vertices;
    public Mesh water;

    void Start()
    {

    }

	void FixedUpdate () {
        if (!enableWaves)
            return;

        Buoyancy buoyancy = GetComponent<Buoyancy>();
       water = GetComponent<MeshFilter>().mesh;
        if (baseHeight == null)
        {
            baseHeight = water.vertices;
        }
        Vector3[] vertices = new Vector3[baseHeight.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = baseHeight[i];
            vertex.y += Mathf.Sin(Time.time * frequency + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * amplitude + initialWaterLevel;
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
        

        water.vertices = vertices;
        water.RecalculateNormals();
        
        MeshCollider waterCollider = GetComponent<MeshCollider>();
        waterCollider.sharedMesh = water;
    
    }

    public Mesh getWaves()
    {
        //Mesh mesh = GetComponent<MeshFilter>().mesh;
        return water;
    }

}
