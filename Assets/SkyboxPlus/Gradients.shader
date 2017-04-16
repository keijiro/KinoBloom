Shader "SkyboxPlus/Gradients"
{
    Properties
    {
        [HDR] _BaseColor("Base Color", Color) = (0.1, 0.1, 0.1)
        [Gamma] _Exposure("Exposure", Range(0, 8)) = 1

        [Toggle] _Switch2("Gradient 2", Float) = 1
        [Toggle] _Switch3("Gradient 3", Float) = 1
        [Toggle] _Switch4("Gradient 4", Float) = 1

        _Direction1("Direction 1", Vector) = (0, 1, 0)
        _Direction2("Direction 2", Vector) = (0.5, 1, 0)
        _Direction3("Direction 3", Vector) = (-0.3, -1, -0.2)
        _Direction4("Direction 4", Vector) = (0, 1, 0)

        [HDR] _Color1("Color 1", Color) = (0.16, 0.18, 0.19)
        [HDR] _Color2("Color 2", Color) = (0.26, 0.28, 0.21)
        [HDR] _Color3("Color 3", Color) = (0.15, 0.15, 0.12)
        [HDR] _Color4("Color 4", Color) = (1.00, 0.99, 0.95)

        _Exponent1("Exponent 1", Range(1, 20)) = 1
        _Exponent2("Exponent 2", Range(1, 20)) = 1
        _Exponent3("Exponent 3", Range(1, 20)) = 1
        _Exponent4("Exponent 4", Range(1, 20)) = 20

        [HideInInspector] _NormalizedVector1("-", Vector) = (0, 1, 0)
        [HideInInspector] _NormalizedVector2("-", Vector) = (0, 1, 0)
        [HideInInspector] _NormalizedVector3("-", Vector) = (0, 1, 0)
        [HideInInspector] _NormalizedVector4("-", Vector) = (0, 1, 0)
    }

    CGINCLUDE

    #pragma shader_feature _SWITCH2_ON
    #pragma shader_feature _SWITCH3_ON
    #pragma shader_feature _SWITCH4_ON

    #include "UnityCG.cginc"

    half3 _BaseColor;
    half _Exposure;

    half3 _NormalizedVector1;
    half3 _NormalizedVector2;
    half3 _NormalizedVector3;
    half3 _NormalizedVector4;

    half3 _Color1;
    half3 _Color2;
    half3 _Color3;
    half3 _Color4;

    half _Exponent1;
    half _Exponent2;
    half _Exponent3;
    half _Exponent4;

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
        half3 d = normalize(i.texcoord.xyz);
        half3 c = _BaseColor;
        c += _Color1 * pow((dot(d, _NormalizedVector1) + 1) * 0.5, _Exponent1);
        #ifdef _SWITCH2_ON
        c += _Color2 * pow((dot(d, _NormalizedVector2) + 1) * 0.5, _Exponent2);
        #endif
        #ifdef _SWITCH3_ON
        c += _Color3 * pow((dot(d, _NormalizedVector3) + 1) * 0.5, _Exponent3);
        #endif
        #ifdef _SWITCH4_ON
        c += _Color4 * pow((dot(d, _NormalizedVector4) + 1) * 0.5, _Exponent4);
        #endif
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
    CustomEditor "SkyboxPlus.GradientsMaterialEditor"
}
