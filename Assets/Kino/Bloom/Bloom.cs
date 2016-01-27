//
// Kino/Bloom v2 - Bloom filter for Unity
//
// Copyright (C) 2015, 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;

namespace Kino
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Kino Image Effects/Bloom")]
    public class Bloom : MonoBehaviour
    {
        #region Public Properties

        /// Prefilter exposure value
        public float exposure {
            get { return _exposure; }
            set { _exposure = value; }
        }

        [SerializeField, Range(0, 1)]
        float _exposure = 0.3f;

        /// Bloom radius
        public float radius {
            get { return _radius; }
            set { _radius = value; }
        }

        [SerializeField, Range(0, 5)]
        float _radius = 2;

        /// Bloom intensity
        public float intensity {
            get { return _intensity; }
            set { _intensity = value; }
        }

        [SerializeField, Range(0, 2)]
        float _intensity = 0.5f;

        /// Quality level option
        public QualityLevel quality {
            get { return _quality; }
            set { _quality = value; }
        }

        [SerializeField]
        QualityLevel _quality = QualityLevel.Normal;

        public enum QualityLevel {
            IgnoreColorSpace, LowResolution, Normal
        }

        /// Anti-flicker median filter
        [SerializeField]
        bool _antiFlicker = false;

        public bool antiFlicker {
            get { return _antiFlicker; }
            set { _antiFlicker = value; }
        }

        #endregion

        #region Private Variables

        [SerializeField, HideInInspector]
        Shader _shader;

        Material _material;

        #endregion

        #region MonoBehaviour Functions

        RenderTexture GetTempBuffer(int width, int height)
        {
            return RenderTexture.GetTemporary(
                width, height, 0, RenderTextureFormat.DefaultHDR);
        }

        void ReleaseTempBuffer(RenderTexture rt)
        {
            RenderTexture.ReleaseTemporary(rt);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            var isGamma = QualitySettings.activeColorSpace == ColorSpace.Gamma;

            // create a materialf for the shader if not yet ready
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            // source texture size (half it when in the low quality mode)
            var tw = source.width;
            var th = source.height;

            if (_quality != QualityLevel.Normal)
            {
                tw /= 2;
                th /= 2;
            }

            // determine the iteration count
            var logh = Mathf.Log(th, 2) + _radius - 6;
            var logh_i = (int)logh;
            var iteration = Mathf.Max(2, logh_i);

            // update the shader properties
            _material.SetFloat("_SampleScale", 0.5f + logh - logh_i);
            _material.SetFloat("_Intensity", _intensity);

            var pf = -Mathf.Log(Mathf.Lerp(1e-2f, 1 - 1e-5f, _exposure), 10);
            _material.SetFloat("_Prefilter", pf * 10);

            if (_antiFlicker)
                _material.EnableKeyword("PREFILTER_MEDIAN");
            else
                _material.DisableKeyword("PREFILTER_MEDIAN");

            if (_quality == QualityLevel.IgnoreColorSpace)
            {
                _material.DisableKeyword("LINEAR_COLOR");
                _material.DisableKeyword("GAMMA_COLOR");
            }
            else if (isGamma)
            {
                _material.DisableKeyword("LINEAR_COLOR");
                _material.EnableKeyword("GAMMA_COLOR");
            }
            else
            {
                _material.EnableKeyword("LINEAR_COLOR");
                _material.DisableKeyword("GAMMA_COLOR");
            }

            // allocate temporary buffers
            var rt1 = new RenderTexture[iteration + 1];
            var rt2 = new RenderTexture[iteration + 1];


            for (var i = 0; i < iteration + 1; i++)
            {
                rt1[i] = GetTempBuffer(tw, th);
                if (i > 0 && i < iteration)
                    rt2[i] = GetTempBuffer(tw, th);
                tw /= 2;
                th /= 2;
            }

            // apply the prefilter
            Graphics.Blit(source, rt1[0], _material, 0);

            // create a mip pyramid
            for (var i = 0; i < iteration; i++)
                Graphics.Blit(rt1[i], rt1[i + 1], _material, 1);

            // blur and combine loop
            _material.SetTexture("_BaseTex", rt1[iteration - 1]);
            Graphics.Blit(rt1[iteration], rt2[iteration - 1], _material, 2);

            for (var i = iteration - 1; i > 1; i--)
            {
                _material.SetTexture("_BaseTex", rt1[i - 1]);
                Graphics.Blit(rt2[i],  rt2[i - 1], _material, 2);
            }

            // finish process
            _material.SetTexture("_BaseTex", source);
            Graphics.Blit(rt2[1], destination, _material, 3);

            // release the temporary buffers
            for (var i = 0; i < iteration + 1; i++)
            {
                ReleaseTempBuffer(rt1[i]);
                ReleaseTempBuffer(rt2[i]);
            }
        }

        #endregion
    }
}
