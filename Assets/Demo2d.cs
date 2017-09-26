using System.Collections;
using UnityEngine;
using komietty.Math;
using vertexAnimator;

public class Demo2d : MonoBehaviour {

    public int lEdge = 20;
    public int nInitialize = 100;
    public int nlimit = 100;
    public int loop = 200;
    public bool isVertexAnimation;
    public GameObject prefab;
    public float pnoiseScale = 10f;
    public float pnoiseAspect = 1f;
    public float threshold = 0;
    public Vector2 pnoiseOrigin = Vector2.zero;
    public GameObject[] prefabArr = new GameObject[0];
    public Texture2D[] Textures = new Texture2D[0];
    Texture2D tex;
    MCMC2d mcmc;

    void Start () {
        Prepare();
        mcmc = new MCMC2d(tex, 0.015f);

        if (isVertexAnimation) StartCoroutine(GenerateWithVertexAnimator());
        else StartCoroutine(Generate());
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

    IEnumerator Generate()
    {
        for (int i = 0; i < loop; i++) // or while(true)
        {
            int rand = (int)Mathf.Floor(Random.value * prefabArr.Length);
            var prefab = prefabArr[rand];
            yield return new WaitForSeconds(0.1f);
            foreach (var pos in mcmc.Sequence(nInitialize, nlimit, threshold))
            {
                var pos_ = pos * lEdge;
                Instantiate(prefab, pos_, Quaternion.identity);
            }
        }
    }

    IEnumerator GenerateWithVertexAnimator()
    {
        for (int i = 0; i < loop; i++) // or while(true)
        {
            int rand = (int)Mathf.Floor(Random.value * Textures.Length);
            var texture = Textures[rand];
            yield return new WaitForSeconds(0.1f);
            foreach (var pos in mcmc.Sequence(nInitialize, nlimit, threshold))
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
