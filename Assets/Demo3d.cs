using System.Collections;
using UnityEngine;
using komietty.Math;
using komietty.flowerBloom;

public class Demo3d : MonoBehaviour
{
    public int lEdge = 20;
    public int nInitialize = 100;
    public int nlimit = 100;
    public GameObject prefab;
    public Texture2D[] mainTextures = new Texture2D[0];
    public Texture2D subTexture;
    Vector4[] data;
    MCMC3d mcmc;

    void Start()
    {
        data = new Vector4[lEdge * lEdge * lEdge];
        Prepare();
        mcmc = new MCMC3d(data, lEdge * Vector3.one);
        StartCoroutine(GenerateFlower());
    }

    void Prepare()
    {
        var sn = new SimplexNoiseGenerator();
        for (int x = 0; x < lEdge; x++)
            for (int y = 0; y < lEdge; y++)
                for (int z = 0; z < lEdge; z++)
                {
                    var i = x + lEdge * y + lEdge * lEdge * z;
                    var val = sn.noise(x, y, z);
                    data[i] = new Vector4(x, y, z, val);
                }
    }

    IEnumerator GenerateFlower()
    {
        for (int i = 0; i < 200; i++)
        {
            int rand = (int)Mathf.Floor(Random.value * mainTextures.Length);
            var mainTexture = mainTextures[rand];
            yield return new WaitForSeconds(0.05f);
            foreach (var pos in mcmc.Sequence(nInitialize, nlimit))
            {
                //yield return new WaitForSeconds(0.01f);
                Quaternion q = Quaternion.Euler(-360 * Random.value, -360 * Random.value, -360 * Random.value);
                GameObject flower = Instantiate(prefab, pos, q);
                FlowerController flowerController = flower.GetComponent<FlowerController>();
                flowerController.InitializeTexture(mainTexture, subTexture);
            }
        }
    }

    IEnumerator Generate()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.05f);
            foreach (var pos in mcmc.Sequence(nInitialize, nlimit))
            {
                //yield return new WaitForSeconds(0.01f);
                Instantiate(prefab, pos, Quaternion.identity);
            }
        }
    }
}
