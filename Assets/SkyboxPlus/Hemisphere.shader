Shader "SkyboxPlus/Hemisphere"
{
    Properties
    {
        [HDR] _TopColor("North Pole", Color) = (0.35, 0.37, 0.42)
        [HDR] _MiddleColor("Equator", Color) = (0.15, 0.15, 0.15)
        [HDR] _BottomColor("South Pole", Color) = (0.12, 0.13, 0.15)
        [Gamma] _Exposure("Exposure", Range(0, 8)) = 1
    }
    CGINCLUDE

    #include "UnityCG.cginc"

    half3 _TopColor;
    half3 _MiddleColor;
    half3 _BottomColor;
    half _Exposure;

    struct appdata_t {
        float4 vertex : POSITION;
    };

    struct v2f {
        float4 vertex : SV_POSITION;
        float3 texcoord : TEXCOORD0;
    };

    v2f vert(appdata_t v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.texcoord = v.vertex.xyz;
        return o;
    }

    half4 frag(v2f i) : SV_Target
    {
        half t1 = max(+i.texcoord.y, 0);
        half t2 = max(-i.texcoord.y, 0);
        half3 c = lerp(lerp(_MiddleColor, _TopColor, t1), _BottomColor, t2);
        return half4(c * _Exposure, 1);
    }

    ENDCG
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    Fallback Off
}
