Shader "Hidden/Kino/FilterTest"
{
    Properties
    {
        _MainTex("", 2D) = "" {}
        _BaseTex("", 2D) = "" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float2 _MainTex_TexelSize;

    sampler2D _BaseTex;
    float2 _BaseTex_TexelSize;

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

    half4 frag_downsample2(v2f_img i) : SV_Target
    {
        float3 d = _MainTex_TexelSize.xyx * float3(1, 1, 0);
        half4 s0 = tex2D(_MainTex, i.uv);
        half4 s1 = tex2D(_MainTex, i.uv - d.xz);
        half4 s2 = tex2D(_MainTex, i.uv + d.xz);
        half4 s3 = tex2D(_MainTex, i.uv - d.zy);
        half4 s4 = tex2D(_MainTex, i.uv + d.zy);

        half4 s = s0 + s1 + s2 - min(min(s0, s1), s2) - max(max(s0, s1), s2);
        s = s + s3 + s4 - min(min(s, s3), s4) - max(max(s, s3), s4);

        s =lerp(s, s0, 0.001);

        half lm = max(0, Luminance(s.rgb));
        lm = smoothstep(0.5, 1.0, lm) * 3;
        return s * lm;
    }

    half4 frag_upscale(v2f_img i) : SV_Target
    {
        half4 m = tex2D(_MainTex, i.uv);
        half4 b = tex2D(_BaseTex, i.uv);
        return m + b;
    }

    ENDCG
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_downsample2
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
            #pragma vertex vert_img
            #pragma fragment frag_upscale
            #pragma target 3.0
            ENDCG
        }
    }
}
