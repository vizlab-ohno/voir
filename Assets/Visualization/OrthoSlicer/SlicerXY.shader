Shader "Custom/OrthoSlicerXYShader"
{
    Properties
    {
    }

    SubShader
    {
        Cull Off
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float4 color : Color;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = IN.color;
            o.Normal = float3(0.0f,1.0f,0.0f);
            o.Metallic = 0.5f;
            o.Smoothness = 0.5f;
            o.Alpha = 1.0f;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
