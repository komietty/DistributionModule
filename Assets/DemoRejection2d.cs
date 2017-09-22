using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using komietty.Math;
using komietty.flowerBloom;

public class DemoRejection2d : MonoBehaviour {

    public int lEdge = 20;
    public int limit = 1000;
    public float threshold = 0.75f;
    public float minLength = 3f;
    public float pnoiseScale = 10f;
    public float pnoiseAspect = 1f;
    public GameObject prefab;
    public Texture2D[] mainTextures = new Texture2D[0];
    public Texture2D subTexture;
    Rejection2d rejection2d;

    struct DistributionData
    {
        public int texindex;
        public Vector3 position;
    }

    List<DistributionData> distributionDataList = new List<DistributionData>();

	void Start () {

        rejection2d = new Rejection2d(Vector2.zero, pnoiseScale, pnoiseAspect);
        StartCoroutine(GenerateFlower());
    }

    IEnumerator GenerateFlower()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.001f);
            foreach (var pos in rejection2d.Sequence(limit, threshold))
            {
                //int texId = whichMainTexIdRandom();
                var pos_ = pos * lEdge;
                int texId = whichMainTexIdClose(pos_, minLength);
                var mainTex = mainTextures[texId];
                Quaternion q = Quaternion.Euler(90, 0, 0);
                GameObject flower = Instantiate(prefab, pos_, q);
                FlowerController flowerController = flower.GetComponent<FlowerController>();
                flowerController.InitializeTexture(mainTex, subTexture);

                var newData = new DistributionData()
                {
                    texindex = texId,
                    position = pos_
                };

                distributionDataList.Add(newData);
            }
        }
    }

    int whichMainTexIdRandom()
    {
        return (int)Mathf.Floor(Random.value * mainTextures.Length);
    }

    int whichMainTexIdClose(Vector2 position, float minLength)
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
        {
            Debug.Log(111);
            return distributionDataList[minLengthIndex].texindex;
        }
        else
        {
            Debug.Log(minLength);
            return (int)Mathf.Floor(Random.value * mainTextures.Length);
        }
            
    }
}
