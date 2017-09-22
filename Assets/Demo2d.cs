using System.Collections;
using UnityEngine;
using komietty.Math;
using komietty.flowerBloom;

public class Demo2d : MonoBehaviour {

    public int lEdge = 20;
    public int nInitialize = 100;
    public int nlimit = 100;
    public GameObject prefab;
    public float pnoiseScale = 10f;
    public float pnoiseAspect = 1f;
    public Vector2 pnoiseOrigin = Vector2.zero;
    public Texture2D[] mainTextures = new Texture2D[0];
    public Texture2D subTexture;
    Texture2D texture;
    MCMC2d mcmc;

    void Start () {
        Prepare();
        mcmc = new MCMC2d(texture, 1f);
        StartCoroutine(Generate());
	}
	
	void Prepare () {
        texture = new Texture2D(128, 128);
        float noiseVal;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                noiseVal = Mathf.PerlinNoise(
                    pnoiseOrigin.x + x * pnoiseScale * pnoiseAspect,
                    pnoiseOrigin.y + y * pnoiseScale);

                Color col = new Color(noiseVal, noiseVal, noiseVal, noiseVal);
                texture.SetPixel(x, y, col);
            }
        }
        texture.Apply();
    }

    IEnumerator Generate()
    {
        for (int i = 0; i < 200; i++)
        {
            int rand = (int)Mathf.Floor(Random.value * mainTextures.Length);
            var mainTexture = mainTextures[rand];
            yield return new WaitForSeconds(0.05f);
            foreach (var pos in mcmc.Sequence(nInitialize, nlimit))
            {
                //yield return new WaitForSeconds(0.01f);
                var pos_ = pos * lEdge;
                Quaternion q = Quaternion.Euler(90, 0, 0);
                GameObject flower = Instantiate(prefab, pos_, q);
                FlowerController flowerController = flower.GetComponent<FlowerController>();
                flowerController.InitializeTexture(mainTexture, subTexture);
            }
        }
    }
}
