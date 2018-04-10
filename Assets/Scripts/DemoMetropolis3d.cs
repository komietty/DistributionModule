using System.Collections;
using UnityEngine;
using MCMC;
using vertexAnimator;

public class DemoMetropolis3d : MonoBehaviour
{
    public int lEdge = 20;
    public int nInitialize = 100;
    public int nlimit = 100;
    public int loop = 400;
    public float threshold = -100;
    public GameObject prefab;
    public Texture2D[] Textures = new Texture2D[0];
    Vector4[] data;
    Metropolis3d metropolis;

    void Start()
    {
        data = Prepare();
        var sn = new SimplexNoiseGenerator();
        metropolis = new Metropolis3d(lEdge * Vector3.one, data, null);
        StartCoroutine(GenerateWithVertexAnimator());
    }

    Vector4[] Prepare()
    {
        var sn = new SimplexNoiseGenerator();
        var distribution = new Vector4[lEdge * lEdge * lEdge];
        for (int x = 0; x < lEdge; x++)
            for (int y = 0; y < lEdge; y++)
                for (int z = 0; z < lEdge; z++)
                {
                    var i = x + lEdge * y + lEdge * lEdge * z;
                    var val = sn.noise(x, y, z);
                    distribution[i] = new Vector4(x, y, z, val);
                }

        return distribution;
    }

    IEnumerator GenerateWithVertexAnimator()
    {
        for (int i = 0; i < loop; i++)
        {
            int rand = (int)Mathf.Floor(Random.value * Textures.Length);
            var texture = Textures[rand];
            yield return new WaitForSeconds(0.05f);
            foreach (var pos in metropolis.Chain(nInitialize, nlimit, threshold))
            {
                Quaternion q = Quaternion.Euler(-360 * Random.value, -360 * Random.value, -360 * Random.value);
                GameObject instance = Instantiate(prefab, pos, q);
                instance.transform.SetParent(transform, false);
                VertexAnimController VAcontroller = instance.GetComponent<VertexAnimController>();
                VAcontroller.InitTexture(texture);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 1);
        Gizmos.DrawWireCube(Vector3.one * lEdge/2, Vector3.one * lEdge);
    }
}
