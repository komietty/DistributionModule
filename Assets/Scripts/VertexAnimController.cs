using System.Collections;
using UnityEngine;

namespace vertexAnimator
{
    public class VertexAnimController : MonoBehaviour
    {
        public float lifeTime = 60f;
        public float lifeTimeCounter = 0f;
        public Vector2 lifeTimeVariationRange = new Vector2(1f, 5f);
        public bool avoidDie;

        public bool animationPause;
        public float animationPauseTime = 20f;
        public float animationPauseTimeCounter = 0f;

        protected Renderer renderer;
        protected MaterialPropertyBlock matPropertyBlock;

        protected virtual void Awake()
        {
            lifeTime *= Random.Range(lifeTimeVariationRange.x, lifeTimeVariationRange.y);
            InitRenderer();
            UpdateVertexAnimator(0);
        }

        protected virtual void Update()
        {
            UpdateLifeTime();
            UpdateVertexAnimator();
        }

        protected virtual void InitRenderer()
        {
            renderer = base.GetComponent<Renderer>();
            matPropertyBlock = new MaterialPropertyBlock();
        }

        public void InitTexture(Texture2D tex)
        {
            const string MAIN_TEX = "_MainTex";
            matPropertyBlock.SetTexture(MAIN_TEX, tex);
        }

        protected virtual void UpdateLifeTime()
        {
            lifeTimeCounter += Time.deltaTime;
            if (lifeTimeCounter > lifeTime && !avoidDie) SetToDie();
        }

        protected virtual void SetToDie()
        {
            avoidDie = false;
            animationPause = false;
        }

        public IEnumerator SetToDieWithWait(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            SetToDie();
        }

        protected virtual void UpdateVertexAnimator()
        {
            if (animationPauseTimeCounter > animationPauseTime && animationPause) return;
            else animationPauseTimeCounter += Time.deltaTime;

            UpdateVertexAnimator(animationPauseTimeCounter);
        }

        protected virtual void UpdateVertexAnimator(float timeEllasped)
        {
            const string ANIMTEX_T = "_AnimTex_T";

            matPropertyBlock.SetFloat(ANIMTEX_T, timeEllasped);
            renderer.SetPropertyBlock(matPropertyBlock);
        }

    }

}