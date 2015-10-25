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
Shader "Hidden/Kino/Bloom"
{
    Properties
    {
        _MainTex("-", 2D) = "" {}
        _BlurTex("-", 2D) = "" {}
        _AccTex("-", 2D) = "" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    #pragma multi_compile _ TEMP_FILTER

    sampler2D _MainTex;
    float2 _MainTex_TexelSize;

    sampler2D _BlurTex;
    sampler2D _AccTex;

    float _Threshold;
    float _Intensity;
    float _TempFilter;

    // Quarter downsampler
    half4 frag_downsample(v2f_img i) : SV_Target
    {
        float4 d = _MainTex_TexelSize.xyxy * float4(1, 1, -1, -1);
        half4 s;
        s  = tex2D(_MainTex, i.uv + d.xy);
        s += tex2D(_MainTex, i.uv + d.xw);
        s += tex2D(_MainTex, i.uv + d.zy);
        s += tex2D(_MainTex, i.uv + d.zw);
        return s * 0.25;
    }

    half4 frag_downsample_last(v2f_img i) : SV_Target
    {
        half4 cs = frag_downsample(i);
        half lm = Luminance(cs.rgb);
        half4 co = cs * smoothstep(_Threshold, _Threshold * 1.5, lm);
#if TEMP_FILTER
        half4 cp = tex2D(_AccTex, i.uv);
        return lerp(co, cp, _TempFilter);
#else
        return co;
#endif
    }

    // 13-tap box filter with linear sampling
    half4 box_filter(float2 uv, float2 stride)
    {
        half4 s = tex2D(_MainTex, uv) / 2;

        float2 d1 = stride * 1.5;
        s += tex2D(_MainTex, uv + d1);
        s += tex2D(_MainTex, uv - d1);

        float2 d2 = stride * 3.5;
        s += tex2D(_MainTex, uv + d2);
        s += tex2D(_MainTex, uv - d2);

        float2 d3 = stride * 5.5;
        s += tex2D(_MainTex, uv + d3);
        s += tex2D(_MainTex, uv - d3);

        return s * 2 / 13;
    }

    // Separable blur filters
    half4 frag_blur_h(v2f_img i) : SV_Target
    {
        return box_filter(i.uv, float2(_MainTex_TexelSize.x, 0));
    }

    half4 frag_blur_v(v2f_img i) : SV_Target
    {
        return box_filter(i.uv, float2(0, _MainTex_TexelSize.y));
    }

    // Composite function
    half4 frag_composite(v2f_img i) : SV_Target
    {
        half4 cs = tex2D(_MainTex, i.uv);
        half3 c1 = LinearToGammaSpace(cs.rgb);
        half3 c2 = LinearToGammaSpace(tex2D(_BlurTex, i.uv).rgb);
        half3 co = GammaToLinearSpace(c1 + c2 * _Intensity);
        return half4(co, cs.a);
    }

    ENDCG
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_downsample
            #pragma target 3.0
            ENDCG
        }
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_downsample_last
            #pragma target 3.0
            ENDCG
        }
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_blur_h
            #pragma target 3.0
            ENDCG
        }
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_blur_v
            #pragma target 3.0
            ENDCG
        }
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_composite
            #pragma target 3.0
            ENDCG
        }
    }
}
