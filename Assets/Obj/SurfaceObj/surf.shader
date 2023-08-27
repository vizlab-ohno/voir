Shader "Unlit/SurfObjShader"
{
    Properties
    {
        _SurfColor("Surface", Color) = (0.5, 0.5, 0.5, 1.0)
        _Shin ("Shininess", Range(0.0, 128.0)) = 10
    }

    SubShader
    {
        Cull Off
        Tags { "RenderType"="Opaque"  "LightMode"="ForwardBase"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            
            fixed4 _SurfColor;
            float _Shin;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal: NORMAL;
                float3 ld: TEXCOORD0;
                float3 ambient: COLOR0;
                float3 vd: TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.ld = -WorldSpaceLightDir(v.vertex);
                o.ambient = ShadeSH9(half4(o.normal, 1));
                o.vd = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 ld = normalize(i.ld);
                float3 normal = normalize(i.normal);
                float3 refL = reflect(i.ld, normal);
                float3 vd = normalize(i.vd);

                float df = dot(ld, normal);
                if(df < 0.0){
                    df = -1.0*df;
                }
                float4 diff = float4(df,df,df,1.0);
                float sp = clamp(dot(vd, refL), 0.0, 1.0);
                float4 spec = pow(sp, _Shin);
                float4 finalColor = (float4(i.ambient, 1.0)+diff+spec)*_LightColor0*_SurfColor;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
