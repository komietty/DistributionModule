using System.Collections;
using UnityEngine;
using komietty.Math;

public class Demo : MonoBehaviour
{

    public int lEdge = 20;
    public int nInitialize = 100;
    public int nlimit = 100;
    public GameObject prefab;
    Material mat;
    Vector4[] data;
    MCMC mcmc;

    void Start()
    {
        data = new Vector4[lEdge * lEdge * lEdge];
        Prepare();
        mcmc = new MCMC(data, lEdge * Vector3.one);
        StartCoroutine(Generate());
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
        while (true)
        {
            yield return new WaitForSeconds(0.001f);
            foreach (var pos in mcmc.Sequence(nInitialize, nlimit))
            {
                //yield return new WaitForSeconds(0.001f);
                Instantiate(prefab, pos, Quaternion.identity);
            }
        }
    }
}
