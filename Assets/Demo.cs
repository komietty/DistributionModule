using System.Collections;
using UnityEngine;
using komietty.Math;

public class Demo : MonoBehaviour {

    public int nDistribution = 10000;
    public int nInitialize = 100;
    public int nlimit = 100;
    public GameObject pointprefab;
    private Vector3[] data;
    private MCMC mcmc;

	void Start () {
        data = new Vector3[nDistribution];
        Prepare();
        mcmc = new MCMC(data);
        StartCoroutine(Generate());
    }

    void Prepare()
    {
        for(int i=0; i< nDistribution; i++)
        {
            var center = (Random.value > 0.5f) ? Vector3.one : Vector3.zero;
            Vector3 pos = 2 * GaussianDistributionCubic.GenerateRandomPointStandard();
            pos.x += 5 * RandomGenerator.rand_lnormal(0f, 1f);
            pos.y += 5 * RandomGenerator.rand_lnormal(0f, 1f);
            pos.z += 5 * RandomGenerator.rand_lnormal(0f, 1f);
            data[i] = pos;
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
                Instantiate(pointprefab, pos, Quaternion.identity);
            }
        }
    }
}
