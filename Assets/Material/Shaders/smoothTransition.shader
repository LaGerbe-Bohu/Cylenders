Shader "Hidden/BOHU/smoothTransition"
{
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

        
            
           struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenuv : TEXCOORD1;
                float3 viewSpaceDir : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = float4(v.vertex.xy, 0.0, 1.0);
                o.uv =  (v.vertex.xy + 1.0) * 0.5;
                #if UNITY_UV_STARTS_AT_TOP
                    o.uv  = o.uv  * float2(1.0, -1.0) + float2(0.0, 1.0);
                #endif
                o.screenuv = ComputeScreenPos(o.vertex);
                return o;
            
            }

            sampler2D _MainTex;
            sampler2D noiseTexture;
            float4 _MainTex_TexelSize;
            bool InTransition;
            float treshold;
            float3 color;
            float timeStep;
            float noiseStrength ;
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv); 
                fixed4 noise = tex2D(noiseTexture, i.uv); 
                
                i.uv -= .5;
                float _Aspect = _MainTex_TexelSize.z/_MainTex_TexelSize.w;   
                i.uv.x *= _Aspect;

                float offset = 0;
                float speed = 4;

                 offset += timeStep+(noise.x*noiseStrength);  
                
                float dist;
                dist = length(i.uv);

                if(!InTransition)
                {
                    dist = 1-smoothstep(offset,offset+treshold,dist);    
                }
                else
                {
                    dist = smoothstep(1-offset,1-offset+treshold,dist);    
                }
                
                float3 output = lerp(color,col,dist);
                return float4(output,1.0);
            }
            ENDCG
        }
                
    }
}
