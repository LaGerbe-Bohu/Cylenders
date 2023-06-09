

Shader "Hidden/CylencerShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
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
            #define MAX_ITERATION 1000
            #define MIN_DELTA 0.01
            #define MAX_DISTANCE 1000
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }


            float sdCylinder(float3 p, float3 a, float3 b, float r) {
                float3 ab = b-a;
                float3 ap = p-a;

                float t = dot(ab, ap) / dot(ab, ab);
          

                float3 c = a + t*ab;
      
                float x = length(p-c)-r;
                float y = (abs(t-.5)-.5)*length(ab);
                float e = length(max(float2(x, y), 0.));
                float i = min(max(x, y), 0.);

                return e+i;
            }


            float RayMarch(float3 origin,float3 direction)
            {
                float step = 0.0;
                for (int i =0;i <MAX_ITERATION;i++)
                {
                    float p = origin + direction*step; 
                    float d = sdCylinder(p,float3(0.0,0.0,1.0),float3(0,1,0),.3);
                    step += d;

                    if(step > MAX_DISTANCE || d < MIN_DELTA ) break;
                }

                return step;
            }
                    
            sampler2D _MainTex;
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = (i.uv-.5)*2;
               
                
                float3 or = float3(0,0,-.1);
                float3 rayon = normalize(float3(uv.x, uv.y, 1));
                float d = RayMarch(or, rayon)/100000.0;

             
                
                return float4(d,d,d,1);
            }
            ENDCG
        }
    }
}
