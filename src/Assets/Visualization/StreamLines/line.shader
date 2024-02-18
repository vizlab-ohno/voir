Shader "Custom/LineShader"
{
    Properties
    {
    }

    SubShader
    {
        Cull Off
        Tags { "RenderType" = "Opaque"  "LightMode" = "ForwardBase"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            float _Shin;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color: COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color: COLOR0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
        FallBack "Diffuse"

}
