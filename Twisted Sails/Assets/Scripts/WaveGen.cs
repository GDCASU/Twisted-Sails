using UnityEngine;
using System.Collections;

/*
Generates waves, just changes the mesh of the water.
Used script found on the unity forms here: http://answers.unity3d.com/questions/443031/sinus-for-rolling-waves.html
as it was better than what I worked on for an hour or two >_>
-Ryan Black


Modified 11/7/16 by Diego Wilde
The wave generator is now turned on, added wave amplitude and phase angle
*/


public class WaveGen : MonoBehaviour
{
    public bool enableWaves = true;
    public float initialWaterLevel = 0; // baseline of wave
    public float amplitude = 0.1f;
    public float frequency = 0.5f;
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;
    public float phase = 0; // in Radians


    private Vector3[] baseHeight;
    public Mesh water;

    void Start()
    {

    }

    void FixedUpdate()
    {
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
            vertex.y += Mathf.Sin(Time.time * frequency + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z + phase) * amplitude + initialWaterLevel;
            vertex.y += Mathf.PerlinNoise(baseHeight[i].x + noiseWalk, baseHeight[i].y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
            vertices[i] = vertex;
        }

        water.vertices = vertices;
        water.RecalculateNormals();

        MeshCollider waterCollider = GetComponent<MeshCollider>();
        waterCollider.sharedMesh = water;

    }

    public Mesh getWaves()
    {
        return water;
    }

}
