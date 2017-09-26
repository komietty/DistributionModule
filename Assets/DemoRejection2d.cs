using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using komietty.Math;
using vertexAnimator;

public class DemoRejection2d : MonoBehaviour {

    public int lEdge = 20;
    public int limit = 1000;
    public int loop = 200;
    public float threshold = 0.75f;
    public float minLength = 3f;
    public float pnoiseScale = 10f;
    public float pnoiseAspect = 1f;
    public bool isVertexAnimation;
    public GameObject prefab;
    public Texture2D[] Textures = new Texture2D[0];
    Rejection2d rejection2d;

    struct DistributionData
    {
        public int texindex;
        public Vector3 position;
    }

    List<DistributionData> distributionDataList = new List<DistributionData>();

	void Start () {

        rejection2d = new Rejection2d(Vector2.zero, pnoiseScale, pnoiseAspect);
        if (isVertexAnimation) StartCoroutine(GenerateWithVertexAnimator());
        else StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        for (int i = 0; i < loop; i++) // or while(true)while (true)
        {
            yield return new WaitForSeconds(0.001f);
            foreach (var pos in rejection2d.Sequence(limit, threshold))
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
            yield return new WaitForSeconds(0.001f);
            foreach (var pos in rejection2d.Sequence(limit, threshold))
            {
                var pos_ = pos * lEdge;
                int texId = WhichTexIdClose(pos_, minLength);
                var texture = Textures[texId];
                Quaternion q = Quaternion.Euler(-90, 0, 0);
                GameObject instance = Instantiate(prefab, pos_, q);
                VertexAnimController VAcontroller = instance.GetComponent<VertexAnimController>();
                VAcontroller.InitTexture(texture);

                var newData = new DistributionData()
                {
                    texindex = texId,
                    position = pos_
                };

                distributionDataList.Add(newData);
            }
        }
    }

    int WhichTexIdRandom()
    {
        return (int)Mathf.Floor(Random.value * Textures.Length);
    }

    int WhichTexIdClose(Vector2 position, float minLength)
    {
        int minLengthIndex = -1;
        for (int i = 0; i < distributionDataList.Count; i++)
        {
            float length = Vector3.SqrMagnitude((Vector3)position - distributionDataList[i].position);

           
            if (length < minLength)
            {
                minLength = length;
                minLengthIndex = i;
            }
        }

        if (minLengthIndex > -1)
            return distributionDataList[minLengthIndex].texindex;
        else
            return (int)Mathf.Floor(Random.value * Textures.Length);
    }
}
