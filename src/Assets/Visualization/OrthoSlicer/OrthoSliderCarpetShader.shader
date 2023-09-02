Shader "Custom/OrthoSliderCarpetShader"
{
    Properties
    {
        _Shin("Shininess", Range(0.0, 128.0)) = 10
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
                float3 normal: NORMAL;
                float4 color: COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal: NORMAL;
                float4 color: COLOR0;
                float3 ld: TEXCOORD0;
                float4 ambient: COLOR1;
                float3 vd: TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.ld = -WorldSpaceLightDir(v.vertex);
                float3 amb3 = ShadeSH9(half4(o.normal, 1));
                o.ambient = float4(amb3, 1.0);
                o.vd = WorldSpaceViewDir(v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = i.normal;
                float3 ld = normalize(i.ld);
                float3 rL = reflect(i.ld, normal);
                float3 vd = normalize(i.vd);
                float4 amb = i.ambient;

                float df = dot(ld, normal);
                if (df < 0.0) {
                    df = -df;
                }
                float4 diff = float4(df,df,df,1.0);
                float sp = clamp(dot(vd, rL), 0.0, 1.0);
                float4 spec = pow(sp, _Shin);
                float4 shadedcol = (amb + diff + spec) * _LightColor0 * i.color;
                return shadedcol;
            }
            ENDCG
        }
    }
        FallBack "Diffuse"

}
