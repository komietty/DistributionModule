using System.Collections.Generic;
using UnityEngine;

namespace komietty.Math
{
    public class Rejection2d
    {
        public Vector2 PnoiseOrigin { get; private set; }
        public float NoiseScale { get; private set; }
        public float NoiseAspect { get; private set; }

        public Rejection2d(Vector2 origin, float scale, float aspect)
        {
            this.PnoiseOrigin = origin;
            this.NoiseScale = scale;
            this.NoiseAspect = aspect;
        }

        public IEnumerable<Vector2> Sequence(int limit, float threshold)
        {
            float randomX;
            float randomY;
            float noiseValue;
            for (int i = 0; i < limit; i++)
            {
                randomX = Random.value;
                randomY = Random.value;
                noiseValue = Mathf.PerlinNoise(
                    PnoiseOrigin.x + randomX * NoiseScale * NoiseAspect,
                    PnoiseOrigin.y + randomY * NoiseScale);

                if (noiseValue > threshold)
                    yield return new Vector2(randomX, randomY);
                else
                    Debug.Log("False");

            }
        }
    }
}
