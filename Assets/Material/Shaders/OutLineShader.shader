Shader "Unlit/OutLineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Thinkness("Thickness",float)=1
        _Color("Color",color)=(0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Pass
        {
          
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Thinkness;
            float3 _Color;
            v2f vert (appdata v)
            {
                v2f o;
               
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, v.normal));
                o.vertex.xyz += normalize(o.normal) * (_Thinkness);
                 float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
            
                return float4(_Color,1);
            }
            ENDCG
        }


    }
    
    FallBack "Diffuse"
}
