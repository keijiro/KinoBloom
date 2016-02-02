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
Shader "Hidden/Kino/Bloom"
{
    Properties
    {
        _MainTex("", 2D) = "" {}
        _BaseTex("", 2D) = "" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    #pragma multi_compile _ PREFILTER_MEDIAN
    #pragma multi_compile LINEAR_COLOR GAMMA_COLOR

    sampler2D _MainTex;
    sampler2D _BaseTex;

    float2 _MainTex_TexelSize;
    float2 _BaseTex_TexelSize;

    float _PrefilterOffs;
    half _Threshold;
    half _Cutoff;
    float _SampleScale;
    half _Intensity;

    // Luma function with Rec.709 HDTV Standard
    half luma(half3 c)
    {
    #if LINEAR_COLOR
        c = LinearToGammaSpace(c);
    #endif
        return dot(c, half3(0.2126, 0.7152, 0.0722));
    }

    // 3-tap median filter
    half3 median(half3 a, half3 b, half3 c)
    {
        return a + b + c - min(min(a, b), c) - max(max(a, b), c);
    }

    // On some GeForce card, we might get extraordinary high value.
    // This might be a bug in the graphics driver or Unity's deferred
    // lighting shader, but anyway we have to cut it off at the moment.
    half3 limit_hdr(half3 c) { return min(c, 65000); }
    half4 limit_hdr(half4 c) { return min(c, 65000); }

    // Downsampler with 4x4 box filter
    half3 downsample_box4x4(float2 uv)
    {
        float4 d = _MainTex_TexelSize.xyxy * float4(-1, -1, +1, +1);

        half3 s;
        s  = tex2D(_MainTex, uv + d.xy).rgb;
        s += tex2D(_MainTex, uv + d.zy).rgb;
        s += tex2D(_MainTex, uv + d.xw).rgb;
        s += tex2D(_MainTex, uv + d.zw).rgb;

        return s * (1.0 / 4);
    }

    // 9-tap bilinear upsampler
    half3 upsample_9tap(float2 uv)
    {
        float4 d = _MainTex_TexelSize.xyxy * float4(1, 1, -1, 0) * _SampleScale;

        half3 s;
        s  = tex2D(_MainTex, uv - d.xy).rgb;
        s += tex2D(_MainTex, uv - d.wy).rgb * 2;
        s += tex2D(_MainTex, uv - d.zy).rgb;

        s += tex2D(_MainTex, uv + d.zw).rgb * 2;
        s += tex2D(_MainTex, uv       ).rgb * 4;
        s += tex2D(_MainTex, uv + d.xw).rgb * 2;

        s += tex2D(_MainTex, uv + d.zy).rgb;
        s += tex2D(_MainTex, uv + d.wy).rgb * 2;
        s += tex2D(_MainTex, uv + d.xy).rgb;

        return s * (1.0 / 16);
    }

    //
    // Vertex shader
    //

    struct v2f_multitex
    {
        float4 pos : SV_POSITION;
        float2 uv_main : TEXCOORD0;
        float2 uv_base : TEXCOORD1;
    };

    v2f_multitex vert_multitex(appdata_full v)
    {
        v2f_multitex o;
        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
        o.uv_main = v.texcoord.xy;
        o.uv_base = v.texcoord.xy;
    #if UNITY_UV_STARTS_AT_TOP
        if (_BaseTex_TexelSize.y < 0.0)
            o.uv_base.y = 1.0 - o.uv_base.y;
    #endif
        return o;
    }

    //
    // fragment shader
    //

    half4 frag_prefilter(v2f_img i) : SV_Target
    {
        float2 uv = i.uv + _MainTex_TexelSize.xy * _PrefilterOffs;
    #if PREFILTER_MEDIAN
        float3 d = _MainTex_TexelSize.xyx * float3(1, 1, 0);

        half4 s0 = limit_hdr(tex2D(_MainTex, uv));
        half3 s1 = limit_hdr(tex2D(_MainTex, uv - d.xz).rgb);
        half3 s2 = limit_hdr(tex2D(_MainTex, uv + d.xz).rgb);
        half3 s3 = limit_hdr(tex2D(_MainTex, uv - d.zy).rgb);
        half3 s4 = limit_hdr(tex2D(_MainTex, uv + d.zy).rgb);

        half3 m = median(median(s0.rgb, s1, s2), s3, s4);
    #else
        half4 s0 = limit_hdr(tex2D(_MainTex, uv));
        half3 m = s0.rgb;
    #endif
        half lm = luma(m);
    #if GAMMA_COLOR
        m = GammaToLinearSpace(m);
    #endif
        m *= saturate((lm - _Threshold) / _Cutoff);

        return half4(m, s0.a);
    }

    half4 frag_downsample(v2f_img i) : SV_Target
    {
        return half4(downsample_box4x4(i.uv), 0);
    }

    half4 frag_upsample(v2f_multitex i) : SV_Target
    {
        half4 base = tex2D(_BaseTex, i.uv_base);
        half3 blur = upsample_9tap(i.uv_main);
        return half4(base.rgb + blur, 0);
    }

    half4 frag_upsample_final(v2f_multitex i) : SV_Target
    {
        half4 base = tex2D(_BaseTex, i.uv_base);
        half3 blur = upsample_9tap(i.uv_main);
    #if GAMMA_COLOR
        base.rgb = GammaToLinearSpace(base.rgb);
    #endif
        half3 cout = base.rgb + blur * _Intensity;
    #if GAMMA_COLOR
        cout = LinearToGammaSpace(cout);
    #endif
        return half4(cout, base.a);
    }

    ENDCG
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_prefilter
            #pragma target 3.0
            ENDCG
        }
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
            #pragma vertex vert_multitex
            #pragma fragment frag_upsample
            #pragma target 3.0
            ENDCG
        }
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_multitex
            #pragma fragment frag_upsample_final
            #pragma target 3.0
            ENDCG
        }
    }
}
