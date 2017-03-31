using UnityEngine;
using System.Collections;

public class CurvedWaterfallManager : MonoBehaviour {

    float waterScrollSpeed = .0025f;
    float sideScrollSpeed = .004f;

    //waterfall water
    MeshRenderer render;
    Vector2 waterOffset;

    //wavy stuff
    Texture[] wavyTex = new Texture[10];
    MeshRenderer wavyBotRender;
    MeshRenderer wavyTopRender;
    int increment = 0;
    int increment2 = 4;

    //side stuff
    Vector2 leftOffset;
    Vector2 rightOffset;
    MeshRenderer leftRender;
    MeshRenderer rightRender;

    // Use this for initialization
    void Start () {
        leftOffset = new Vector2(0, 0);
        rightOffset = new Vector2(0, .5f);

        render = GetComponent<MeshRenderer>();
        wavyBotRender = transform.GetChild(0).GetComponent<MeshRenderer>();
        leftRender = transform.GetChild(1).GetComponent<MeshRenderer>();
        rightRender = transform.GetChild(2).GetComponent<MeshRenderer>();
        wavyTopRender = transform.GetChild(3).GetComponent<MeshRenderer>();

        for (int i = 0; i < 10; i++)
            wavyTex[i] = Resources.Load("WaterfallFrames/Wavy" + i) as Texture;

        StartCoroutine(FrameUpdate());
    }
	
	// Update is called once per frame
	void Update () {
        waterOffset = new Vector2(waterOffset.x, waterOffset.y + waterScrollSpeed);
        render.material.SetTextureOffset("_LatticePatternTex", waterOffset);

        leftOffset = new Vector2(leftOffset.x, leftOffset.y + sideScrollSpeed);
        leftRender.material.SetTextureOffset("_MainTex", leftOffset);

        rightOffset = new Vector2(rightOffset.x, rightOffset.y + sideScrollSpeed);
        rightRender.material.SetTextureOffset("_MainTex", rightOffset);
    }

    IEnumerator FrameUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            increment++;
            increment2++;
            if (increment > 9)
                increment = 0;
            if(increment2 > 9)
                increment2 = 0;
            wavyBotRender.material.SetTexture("_MainTex", wavyTex[increment]);
            wavyTopRender.material.SetTexture("_MainTex", wavyTex[increment2]);
            Debug.Log(increment2);
        }
    }
}
