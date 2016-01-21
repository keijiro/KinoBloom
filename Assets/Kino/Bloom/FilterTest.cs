using UnityEngine;

namespace Kino
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class FilterTest : MonoBehaviour
    {
        [SerializeField] Shader _shader;
        Material _material;

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
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            var sw = source.width;
            var sh = source.height;

            var rt0 = GetTempBuffer(sw, sh);

            var rt1 = GetTempBuffer(sw / 2, sh / 2);
            var rt2 = GetTempBuffer(sw / 4, sh / 4);
            var rt3 = GetTempBuffer(sw / 8, sh / 8);
            var rt4 = GetTempBuffer(sw / 16, sh / 16);

            var rt1b = GetTempBuffer(sw / 2, sh / 2);
            var rt2b = GetTempBuffer(sw / 4, sh / 4);
            var rt3b = GetTempBuffer(sw / 8, sh / 8);

            Graphics.Blit(source, rt0, _material, 0);

            Graphics.Blit(rt0, rt1, _material, 1);
            Graphics.Blit(rt1, rt2, _material, 1);
            Graphics.Blit(rt2, rt3, _material, 1);
            Graphics.Blit(rt3, rt4, _material, 1);

            _material.SetTexture("_BaseTex", rt3);
            Graphics.Blit(rt4, rt3b, _material, 2);

            _material.SetTexture("_BaseTex", rt2);
            Graphics.Blit(rt3b, rt2b, _material, 2);

            _material.SetTexture("_BaseTex", rt1);
            Graphics.Blit(rt2b, rt1b, _material, 2);

            _material.SetTexture("_BaseTex", source);
            Graphics.Blit(rt1b, destination, _material, 2);

            ReleaseTempBuffer(rt0);
            ReleaseTempBuffer(rt1);
            ReleaseTempBuffer(rt2);
            ReleaseTempBuffer(rt3);
            ReleaseTempBuffer(rt4);

            ReleaseTempBuffer(rt1b);
            ReleaseTempBuffer(rt2b);
            ReleaseTempBuffer(rt3b);
        }
    }
}
