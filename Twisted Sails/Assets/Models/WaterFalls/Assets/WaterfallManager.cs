using UnityEngine;
using System.Collections;

public class WaterfallManager : MonoBehaviour {

    float waterScrollSpeed = .0075f;
    float sideScrollSpeed = .02f;

    //waterfall water
    MeshRenderer waveRender;
    Vector2 waterOffset;

    //wavy stuff
    Texture[] wavyTex = new Texture[10];
    MeshRenderer wavyRender;
    int increment = 0;

    //side stuff
	Vector2 sideOffset;
    MeshRenderer sideRender;

	//spiral stuff
	GameObject spiral;

    // Use this for initialization
    void Start () {
		spiral = transform.GetChild (4).gameObject;

        sideOffset = new Vector2(0, 0);

		waveRender = transform.GetChild(1).GetComponent<MeshRenderer>();
        wavyRender = transform.GetChild(3).GetComponent<MeshRenderer>();
        sideRender = transform.GetChild(2).GetComponent<MeshRenderer>();
        
        for (int i = 0; i < 10; i++)
            wavyTex[i] = Resources.Load("WaterfallFrames/Wavy" + i) as Texture;

        StartCoroutine(FrameUpdate());
    }
	
	// Update is called once per frame
	void Update () {
        waterOffset = new Vector2(waterOffset.x, waterOffset.y + waterScrollSpeed);
        waveRender.material.SetTextureOffset("_LatticePatternTex", waterOffset);

        sideOffset = new Vector2(sideOffset.x, sideOffset.y + sideScrollSpeed);
		sideRender.material.SetTextureOffset ("_MainTex", sideOffset);

		spiral.transform.Rotate (new Vector3 (0, 0, -2.5f));
    }

    IEnumerator FrameUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            increment++;
            if (increment > 9)
                increment = 0;
            wavyRender.material.SetTexture("_MainTex", wavyTex[increment]);
        }
    }
}
