using UnityEngine;
using System.Collections;

namespace komietty.flowerBloom
{
    /// <summary>
    /// 花単体の挙動を制御します。
    /// </summary>
    public class FlowerController : MonoBehaviour
    {
        #region Field

        #region LifeTime Settings

        /// <summary>
        /// 花の寿命(散り始めるまでの時間)。
        /// </summary>
        public float lifeTimeSec = 60;

        /// <summary>
        /// 花の寿命のカウンタ。
        /// </summary>
        [SerializeField] // for Debug.
        protected float lifeTimeSecCounter = 0;

        /// <summary>
        /// 花の寿命のランダム性。
        /// </summary>
        public Vector2 lifeTimeSecRandomRange = new Vector2(1f, 2.5f);

        /// <summary>
        /// 花が寿命で死なないようにする。
        /// true のとき、花の寿命で死にません。
        /// </summary>
        public bool avoidDieFromLifeTimeSec;

        #endregion LifeTime Settings

        #region Animation Settings

        /// <summary>
        /// アニメーションを一時停止するとき true.
        /// </summary>
        public bool animationPause;

        /// <summary>
        /// アニメーションを停止する時間(秒)。
        /// </summary>
        public float animationPauseTimeSec = 18f;

        /// <summary>
        /// アニメーション時間(秒)。
        /// </summary>
        public float animationTimeSec = 60;

        /// <summary>
        /// アニメーションの経過時間(秒)。
        /// </summary>
        public float animationElapsedTimeSecCounter;

        #endregion Animation Settings

        #region Wave Settings

        /// <summary>
        /// 揺れの強さ。
        /// </summary>
        public float wavePower = 0.05f;

        /// <summary>
        /// 揺れの方向。
        /// </summary>
        public Vector4 waveDirection = new Vector4(1, 0, 0, 0);

        #endregion Wave Settings

        #region Renderer Settings

        /// <summary>
        /// 花のレンダラ。
        /// </summary>
        protected Renderer flowerRenderer;

        /// <summary>
        /// 花のレンダラに設定する MaterialPropertyBlock.
        /// </summary>
        protected MaterialPropertyBlock flowerMaterialPropertyBlock;

        #endregion Renderer Settings

        #region TextureLerp Settings

        /// <summary>
        /// テクスチャを変化させる時間。
        /// </summary>
        public float textureLerpDurationTimeSec = 3;

        /// <summary>
        /// テクスチャの変化の方向を反転するかどうか。
        /// 通常はメインテクスチャの比率が徐々に上がりますが、
        /// 有効にするとき、メインテクスチャの比率が徐々に下がります。
        /// </summary>
        public bool invertTextureLerpDirection = false;

        /// <summary>
        /// テクスチャの変化を開始したかどうか。
        /// </summary>
        protected bool textureLerpStartOnce;

        protected bool textureLerpEndOnce;

        protected float textureLerpStartTimeSec;

        #endregion TextureLerp Settings

        #endregion Field

        public bool IsDied
        {
            get { return this.animationElapsedTimeSecCounter > this.animationTimeSec; }
        }

        #region Method

        protected virtual void Awake()
        {
            this.lifeTimeSec *= Random.Range(this.lifeTimeSecRandomRange.x, this.lifeTimeSecRandomRange.y);
            InitializeRenderer();
            InitializeWave();
            UpdateVertexAnimator(0);

            if (this.invertTextureLerpDirection) UpdateTextureLerp(1);
            else UpdateTextureLerp(0);
        }

        /// <summary>
        /// 更新時に呼び出されます。
        /// </summary>
        protected virtual void Update()
        {
            // 実行順が重要な点に注意してください。
            UpdateLifeTime();
            UpdateVertexAnimator();
            UpdateTextureLerp();
            UpdateWave();
        }

        #region Initialize

        /// <summary>
        /// レンダラと関連する設定を初期化します。
        /// </summary>
        protected virtual void InitializeRenderer()
        {
            this.flowerRenderer = base.GetComponent<Renderer>();
            this.flowerMaterialPropertyBlock = new MaterialPropertyBlock();
        }

        /// <summary>
        /// テクスチャを設定します。
        /// </summary>
        /// <param name="mainTexture">
        /// 補助のテクスチャ。
        /// </param>
        /// <param name="subTexture">
        /// サブのテクスチャ。規定値は null です。
        /// null のとき、主要なテクスチャが設定されます。
        /// </param>
        public virtual void InitializeTexture(Texture2D mainTexture, Texture2D subTexture)
        {
            const string MAIN_TEX = "_MainTex";
            const string SUB_TEX = "_SubTex";

            this.flowerMaterialPropertyBlock.SetTexture(MAIN_TEX, mainTexture);
            this.flowerMaterialPropertyBlock.SetTexture(SUB_TEX, subTexture);
        }

        #endregion Initialize

        #region Life

        /// <summary>
        /// 寿命を進めて、アニメーションの静止やループを解除します。
        /// </summary>
        protected virtual void UpdateLifeTime()
        {
            this.lifeTimeSecCounter += Time.deltaTime;

            if (this.lifeTimeSec < this.lifeTimeSecCounter && !this.avoidDieFromLifeTimeSec)
            {
                SetToDie();
            }
        }

        /// <summary>
        /// 死ぬように設定します。
        /// </summary>
        public virtual void SetToDie()
        {
            this.avoidDieFromLifeTimeSec = false;
            this.animationPause = false;
        }

        /// <summary>
        /// 待機時間を付けて死ぬように設定します。
        /// </summary>
        /// <param name="waitTimeSec">
        /// 待機時間(秒)。
        /// </param>
        /// <returns>
        /// IEnumerator.
        /// </returns>
        public IEnumerator SetToDieWithWaitTime(float waitTimeSec)
        {
            yield return new WaitForSeconds(waitTimeSec);
            SetToDie();
        }

        #endregion Life

        #region VertexAnimator

        /// <summary>
        /// VertexAnimator の設定を更新します。
        /// </summary>
        protected virtual void UpdateVertexAnimator()
        {
            if (this.animationPauseTimeSec < this.animationElapsedTimeSecCounter && this.animationPause)
            {
                return;
            }
            else
            {
                this.animationElapsedTimeSecCounter += Time.deltaTime;
            }

            UpdateVertexAnimator(this.animationElapsedTimeSecCounter);
        }

        /// <summary>
        /// VertexAnimator の設定を更新します。
        /// </summary>
        /// <param name="ellapsedTime">
        /// 経過時間。
        /// </param>
        protected virtual void UpdateVertexAnimator(float ellapsedTime)
        {
            const string ANIMTEX_T = "_AnimTex_T";

            this.flowerMaterialPropertyBlock.SetFloat(ANIMTEX_T, ellapsedTime);
            this.flowerRenderer.SetPropertyBlock(this.flowerMaterialPropertyBlock);
        }

        #endregion VertexAnimator

        #region Texture Lerp

        /// <summary>
        /// テクスチャの変化を開始します。
        /// </summary>
        public virtual void StartTextureLerp()
        {
            this.textureLerpStartOnce = true;
            this.textureLerpStartTimeSec = Time.timeSinceLevelLoad;
        }

        /// <summary>
        /// 一定時間待機してからテクスチャの変化を介します。
        /// </summary>
        /// <param name="waitTimeSec">
        /// 待機時間(秒)。
        /// </param>
        /// <returns>
        /// IEnumerator.
        /// </returns>
        public IEnumerator StartTextureLerpWithWaitTime(float waitTimeSec)
        {
            yield return new WaitForSeconds(waitTimeSec);
            StartTextureLerp();
        }

        [SerializeField] // for Debug.
        protected float textureLerpRatio;

        /// <summary>
        /// テクスチャの変化を更新します。
        /// </summary>
        protected virtual void UpdateTextureLerp()
        {
            if (!this.textureLerpStartOnce || this.textureLerpEndOnce)
            {
                return;
            }

            float textureLerpRatio =
                Mathf.Clamp01((Time.timeSinceLevelLoad - this.textureLerpStartTimeSec)
                               / this.textureLerpDurationTimeSec);

            if (this.invertTextureLerpDirection)
            {
                textureLerpRatio = 1 - textureLerpRatio;
            }

            UpdateTextureLerp(textureLerpRatio);

            if (1 <= textureLerpRatio)
            {
                this.textureLerpEndOnce = true;
            }

            // Debug.

            this.textureLerpRatio = textureLerpRatio;
        }

        // 初期化時など、外部から強制定期に変更する需要があるため public の必要があります。

        /// <summary>
        /// テクスチャの変化を更新します。
        /// </summary>
        /// <param name="lerpRatio">
        /// 変化の度合い。1 のとき完全にメインテクスチャになります。
        /// </param>
        public virtual void UpdateTextureLerp(float lerpRatio)
        {
            const string ANIMTEX_T = "_MainTexRatio";

            this.flowerMaterialPropertyBlock.SetFloat(ANIMTEX_T, lerpRatio);

            this.flowerRenderer.SetPropertyBlock(this.flowerMaterialPropertyBlock);
        }

        #endregion Texture Lerp

        #region Wave Motion

        /// <summary>
        /// 揺れの設定を初期化します。
        /// </summary>
        protected virtual void InitializeWave()
        {
            const string WAVE_DIRECTION = "_WaveDirection";
            const string WAVE_POWER = "_WavePower";

            this.flowerMaterialPropertyBlock.SetVector(WAVE_DIRECTION, this.waveDirection);
            this.flowerMaterialPropertyBlock.SetFloat(WAVE_POWER, this.wavePower);
            this.flowerRenderer.SetPropertyBlock(this.flowerMaterialPropertyBlock);
        }

        /// <summary>
        /// 揺れの設定を更新します。
        /// </summary>
        protected virtual void UpdateWave()
        {
            const string WAVE_ELLAPSED_TIME = "_WaveEllapsedTime";

            // 散るアニメーションに入っているときは揺れさせません(散った花びらが揺れてしまう)。
            if (this.animationPauseTimeSec < this.animationElapsedTimeSecCounter && !this.animationPause)
            {
                return;
            }

            this.flowerMaterialPropertyBlock.SetFloat(WAVE_ELLAPSED_TIME, this.lifeTimeSecCounter);
            this.flowerRenderer.SetPropertyBlock(this.flowerMaterialPropertyBlock);
        }

        #endregion Wave Motion

        #endregion Method
    }
}