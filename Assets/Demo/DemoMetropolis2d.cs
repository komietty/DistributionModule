using System.Collections;
using UnityEngine;
using MCMC;
using vertexAnimator;

public class DemoMetropolis2d : MonoBehaviour {

    public int lEdge = 20;
    public int nInitialize = 100;
    public int nlimit = 100;
    public int loop = 200;
    public GameObject prefab;
    public float pnoiseScale = 10f;
    public float pnoiseAspect = 1f;
    public float threshold = 0;
    public Vector2 pnoiseOrigin = Vector2.zero;
    public Texture2D[] Textures = new Texture2D[0];
    Texture2D tex;
    Metropolis2d metropolis;

    void Start () {
        Prepare();
        metropolis = new Metropolis2d(tex, 0.015f);
        StartCoroutine(GenerateWithVertexAnimator());
	}
	
	void Prepare () {
        tex = new Texture2D(128, 128);
        float noiseVal;
        float w = tex.width;
        float h = tex.height;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                noiseVal = Mathf.PerlinNoise(
                    pnoiseOrigin.x + x / w * pnoiseScale * pnoiseAspect,
                    pnoiseOrigin.y + y / h * pnoiseScale);

                Color col = new Color(noiseVal, noiseVal, noiseVal, noiseVal);
                tex.SetPixel(x, y, col);
            }
        }
        tex.Apply();
    }

    IEnumerator GenerateWithVertexAnimator()
    {
        for (int i = 0; i < loop; i++) // or while(true)
        {
            int rand = (int)Mathf.Floor(Random.value * Textures.Length);
            var texture = Textures[rand];
            yield return new WaitForSeconds(0.1f);
            foreach (var pos in metropolis.Chain(nInitialize, nlimit, threshold))
            {
                var pos_ = pos * lEdge;
                Quaternion q = Quaternion.Euler(-90, 0, 0);
                GameObject instance = Instantiate(prefab, pos_, q);
                VertexAnimController VAcontroller = instance.GetComponent<VertexAnimController>();
                VAcontroller.InitTexture(texture);
            }
        }
    }
}
