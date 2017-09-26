using System.Collections;
using UnityEngine;
using UnityEditor;
using komietty.Math;
using vertexAnimator;

public class Demo3d : MonoBehaviour
{
    public int lEdge = 20;
    public int nInitialize = 100;
    public int nlimit = 100;
    public int loop = 400;
    public float threshold = -100;
    public bool isVertexAnimation;
    public GameObject prefab;
    public GameObject[] prefabArr = new GameObject[0];
    public Texture2D[] Textures = new Texture2D[0];
    Vector4[] data;
    MCMC3d mcmc;

    void Start()
    {
        data = new Vector4[lEdge * lEdge * lEdge];
        Prepare();
        mcmc = new MCMC3d(data, lEdge * Vector3.one);
        if (isVertexAnimation) StartCoroutine(GenerateWithVertexAnimator());
        else StartCoroutine(Generate());
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

    IEnumerator Generate()
    {
        for (int i = 0; i < loop; i++) // or while(true)
        {
            int rand = (int)Mathf.Floor(Random.value * prefabArr.Length);
            var prefab = prefabArr[rand];
            yield return new WaitForSeconds(0.05f);
            foreach (var pos in mcmc.Sequence(nInitialize, nlimit, threshold))
            {
                Instantiate(prefab, pos, Quaternion.identity);
            }
        }
    }

    IEnumerator GenerateWithVertexAnimator()
    {
        for (int i = 0; i < loop; i++) // or while(true)
        {
            int rand = (int)Mathf.Floor(Random.value * Textures.Length);
            var texture = Textures[rand];
            yield return new WaitForSeconds(0.05f);
            foreach (var pos in mcmc.Sequence(nInitialize, nlimit, threshold))
            {
                Quaternion q = Quaternion.Euler(-360 * Random.value, -360 * Random.value, -360 * Random.value);
                GameObject instance = Instantiate(prefab, pos, q);
                VertexAnimController VAcontroller = instance.GetComponent<VertexAnimController>();
                VAcontroller.InitTexture(texture);
            }
        }
    }
}
