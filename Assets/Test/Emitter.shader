Shader "Custom/Emitter"
{
    Properties
    {
        [HDR] _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface surf Standard
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half4 _Color;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Emission = _Color * (0.5 + 0.5 * cos(_Time.y));
        }

        ENDCG
    }
    FallBack "Diffuse"
}
