Shader "Custom/LocalSlicerShader"
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
                float4 shadedcol = _LightColor0 * i.color;
                return shadedcol;
            }
            ENDCG
        }
    }
        FallBack "Diffuse"
}
