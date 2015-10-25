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

        // Bloom radius
        [SerializeField, Range(1, 10)]
        float _radius1 = 1.0f;

        public float radius1 {
            get { return _radius1; }
            set { _radius1 = value; }
        }

        [SerializeField, Range(1, 10)]
        float _radius2 = 4.0f;

        public float radius2 {
            get { return _radius2; }
            set { _radius2 = value; }
        }

        // Bloom intensity
        [SerializeField, Range(0, 1.0f)]
        float _intensity1 = 0.2f;

        public float intensity1 {
            get { return _intensity1; }
            set { _intensity1 = value; }
        }

        [SerializeField, Range(0, 1.0f)]
        float _intensity2 = 0.2f;

        public float intensity2 {
            get { return _intensity2; }
            set { _intensity2 = value; }
        }

        // Threshold
        [SerializeField, Range(0, 2)]
        float _threshold = 0.5f;

        public float threshold {
            get { return _threshold; }
            set { _threshold = value; }
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

        RenderTexture GetTempBuffer(int width, int height)
        {
            return RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.DefaultHDR);
        }

        void ReleaseTempBuffer(RenderTexture rt)
        {
            RenderTexture.ReleaseTemporary(rt);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // Set up the material object.
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            // Common options
            _material.SetFloat("_Intensity1", _intensity1);
            _material.SetFloat("_Intensity2", _intensity2);
            _material.SetFloat("_Threshold", _threshold);
            _material.SetFloat("_TempFilter", 1.0f - Mathf.Exp(_temporalFiltering * -4));

            // Calculate the size of the blur buffers.
            var blur1Height = (int)(720 / _radius1);
            var blur2Height = (int)(256 / _radius2);
            var blur1Width = blur1Height * source.width / source.height;
            var blur2Width = blur2Height * source.width / source.height;

            // Allocate the blur buffers.
            var rt1 = GetTempBuffer(blur1Width, blur1Height);
            var rt2 = GetTempBuffer(blur1Width, blur1Height);
            var rt3 = GetTempBuffer(blur2Width, blur2Height);
            var rt4 = GetTempBuffer(blur2Width, blur2Height);

            // Small bloom: shrink the source image and apply the threshold.
            RenderTexture rt = source;

            while (rt.height > blur1Height * 4) // quater downsampling
            {
                var rt_next = GetTempBuffer(rt.width / 4, rt.height / 4);
                Graphics.Blit(rt, rt_next, _material, 0);
                if (rt != source) ReleaseTempBuffer(rt);
                rt = rt_next;
            }

            Graphics.Blit(rt, rt1, _material, 1); // thresholding + downsampling

            if (rt != source) ReleaseTempBuffer(rt);

            // Large bloom: shrink, threshold and temporal filtering
            rt = source;

            while (rt.height > blur2Height * 4) // quater downsampling
            {
                var rt_next = GetTempBuffer(rt.width / 4, rt.height / 4);
                Graphics.Blit(rt, rt_next, _material, 0);
                if (rt != source) ReleaseTempBuffer(rt);
                rt = rt_next;
            }

            if (_accBuffer && _temporalFiltering > 0.0f)
            {
                // Temporal filtering + thresholding + downsampling
                _material.SetTexture("_AccTex", _accBuffer);
                Graphics.Blit(rt, rt3, _material, 2);
            }
            else
            {
                // Thresholding + downsampling
                Graphics.Blit(rt, rt3, _material, 1);
            }

            if (rt != source) ReleaseTempBuffer(rt);

            // Update the accmulation buffer.
            if (_accBuffer)
            {
                ReleaseTempBuffer(_accBuffer);
                _accBuffer = null;
            }

            if (_temporalFiltering > 0.0f)
            {
                _accBuffer = GetTempBuffer(blur2Width, blur2Height);
                Graphics.Blit(rt3, _accBuffer);
            }

            // Small bloom: apply the separable Gaussian filter.
            for (var i = 0; i < 2; i++)
            {
                Graphics.Blit(rt1, rt2, _material, 3); // horizontal
                Graphics.Blit(rt2, rt1, _material, 4); // vertical
            }

            // Large bloom: apply the separable box filter repeatedly.
            for (var i = 0; i < 4; i++)
            {
                Graphics.Blit(rt3, rt4, _material, 5); // horizontal
                Graphics.Blit(rt4, rt3, _material, 6); // vertical
            }

            // Compositing
            _material.SetTexture("_Blur1Tex", rt1);
            _material.SetTexture("_Blur2Tex", rt3);
            Graphics.Blit(source, destination, _material, 7);

            // Release the blur buffers.
            ReleaseTempBuffer(rt1);
            ReleaseTempBuffer(rt2);
            ReleaseTempBuffer(rt3);
            ReleaseTempBuffer(rt4);
        }

        #endregion
    }
}
