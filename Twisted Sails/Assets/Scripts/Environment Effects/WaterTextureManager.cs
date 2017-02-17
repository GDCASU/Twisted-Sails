using UnityEngine;
using System.Collections;

public class WaterTextureManager : MonoBehaviour
{

    Texture[] waveTex = new Texture[200];
    MeshRenderer render;
    int increment1 = 0;
    float increment2 = 0;

    // Use this for initialization
    void Start()
    {
        render = GetComponent<MeshRenderer>();

        for (int i = 0; i < 200; i++)
        {
            if (i <= 9)
                waveTex[i] = Resources.Load("MovieFrames/WaterAnimation_0000" + i) as Texture;
            else if( i <= 99)
                waveTex[i] = Resources.Load("MovieFrames/WaterAnimation_000" + i) as Texture;
            else
                waveTex[i] = Resources.Load("MovieFrames/WaterAnimation_00" + i) as Texture;
        }

        StartCoroutine(Frame1Update());
        StartCoroutine(Frame2Update());
    }

    IEnumerator Frame1Update()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            increment1++;
            if (increment1 > 199)
                increment1 = 0;
            render.material.SetTexture("_MainTex", waveTex[increment1]);
        }
    }

    IEnumerator Frame2Update()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            increment2 += .0002f;
            if (increment2 >= 1)
                increment2 = 0;
            render.material.SetTextureOffset("_MainTex2", new Vector2(increment2, increment2));
        }
    }
}