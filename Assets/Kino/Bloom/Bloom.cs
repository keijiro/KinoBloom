//
// KinoBloom - Bloom effect
//
// Copyright (C) 2015 Keijiro Takahashi
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

        // Radius
        [SerializeField, Range(2, 40)]
        float _radius = 10.0f;

        public float radius {
            get { return _radius; }
            set { _radius = value; }
        }

        // Threshold
        [SerializeField, Range(0, 2)]
        float _threshold = 0.5f;

        public float threshold {
            get { return _threshold; }
            set { _threshold = value; }
        }

        // Intensity
        [SerializeField, Range(0, 1.0f)]
        float _intensity = 0.2f;

        public float intensity {
            get { return _intensity; }
            set { _intensity = value; }
        }

        // Temporal filtering
        [SerializeField, Range(0, 1.0f)]
        float _temporalFiltering = 0.0f;

        public float temporalFiltering {
            get { return _temporalFiltering; }
            set { _temporalFiltering = value; }
        }

        #endregion

        #region Private Properties

        [SerializeField] Shader _shader;
        Material _material;
        RenderTexture _accBuffer;

        #endregion

        #region MonoBehaviour Functions

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // Always use an HDR texture.
            var fmt = RenderTextureFormat.DefaultHDR;

            // Width/height of the blur buffer
            var blurHeight = (int)(source.width / _radius);
            var blurWidth = blurHeight * source.width / source.height;

            // Set up the material object.
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            _material.SetFloat("_Threshold", _threshold);
            _material.SetFloat("_Intensity", _intensity);

            // Shrink the source image with the quarter downsampler.
            RenderTexture rt = source;
            while (rt.height > blurHeight * 4)
            {
                var rt_next = RenderTexture.GetTemporary(rt.width / 4, rt.height / 4, 0, fmt);
                Graphics.Blit(rt, rt_next, _material, 0);
                if (rt != source) RenderTexture.ReleaseTemporary(rt);
                rt = rt_next;
            }

            // Double blur buffer
            var rt1 = RenderTexture.GetTemporary(blurWidth, blurHeight, 0, fmt);
            var rt2 = RenderTexture.GetTemporary(blurWidth, blurHeight, 0, fmt);

            // Enable the temporal filter if available.
            if (_accBuffer && _temporalFiltering > 0.0f)
            {
                _material.EnableKeyword("TEMP_FILTER");
                _material.SetTexture("_AccTex", _accBuffer);
                var coeff = 1.0f - Mathf.Exp(_temporalFiltering * -4);
                _material.SetFloat("_TempFilter", coeff);
            }
            else
            {
                _material.DisableKeyword("TEMP_FILTER");
            }

            // Shrink to the size of the blur buffer with the downsampler.
            // This time it applies the threshold function.
            Graphics.Blit(rt, rt1, _material, 1);
            if (rt != source) RenderTexture.ReleaseTemporary(rt);

            // The accumulation buffer is no longer needed.
            if (_accBuffer)
            {
                RenderTexture.ReleaseTemporary(_accBuffer);
                _accBuffer = null;
            }

            // Make a copy of the shrinked image.
            if (_temporalFiltering > 0.0f)
            {
                _accBuffer = RenderTexture.GetTemporary(blurWidth, blurHeight, 0, fmt);
                Graphics.Blit(rt1, _accBuffer);
            }

            // Apply the separable box filter repeatedly.
            for (var i = 0; i < 4; i++)
            {
                Graphics.Blit(rt1, rt2, _material, 2); // horizontal
                Graphics.Blit(rt2, rt1, _material, 3); // vertical
            }

            // Compositing
            _material.SetTexture("_BlurTex", rt1);
            Graphics.Blit(source, destination, _material, 4);

            // Release the blur buffers.
            RenderTexture.ReleaseTemporary(rt1);
            RenderTexture.ReleaseTemporary(rt2);
        }

        #endregion
    }
}
