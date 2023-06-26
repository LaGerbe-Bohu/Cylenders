

Shader "Hidden/CylencerShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BackgroundColor("BackgroundColor",color)=(0,0,0,0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Blend One Zero
        
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


            float sdCylinder(float3 p, float3 a, float3 b, float r)
            {
                float3  ba = b - a;
                float3  pa = p - a;
                float baba = dot(ba,ba);
                float paba = dot(pa,ba);
                float x = length(pa*baba-ba*paba) - r*baba;
                float y = abs(paba-baba*0.5)-baba*0.5;
                float x2 = x*x;
                float y2 = y*y*baba;
                
                float d = (max(x,y)<0.0)?-min(x2,y2):(((x>0.0)?x2:0.0)+((y>0.0)?y2:0.0));
                
                return sign(d)*sqrt(abs(d))/baba;
            }
            
            float sdSphere( float3 p,float3 c, float s )
            {
              return distance(p, (c-s));
            }


            float map( in float3 pos )
            {
                return sdCylinder(pos,float3(0.0,-cos(_Time.z)*2-10,0.0),float3(0.0,cos(_Time.z)*2+10,0.0),cos(_Time.z)+2);
            }

            float3 calcNormal( in float3 pos )
            {
                float2 e = float2(1.0,-1.0)*0.5773;
                const float eps = 0.0005;
                return normalize( e.xyy*map( pos + e.xyy*eps ) + 
					              e.yyx*map( pos + e.yyx*eps ) + 
					              e.yxy*map( pos + e.yxy*eps ) + 
					              e.xxx*map( pos + e.xxx*eps ) );
            }
            
            sampler2D _MainTex;
            float4 _BackgroundColor;
            
            fixed4 frag (v2f i) : SV_Target
            {

                float3 tex = tex2D(_MainTex,i.uv);
                float2 uv = (i.uv-.5);
               
                
                float3 or = float3(0,0,-30);
                float3 rayon = normalize(float3(uv.x, uv.y, 1));
          
                
          
                float t = 0.0;
             
                for( int i=0; i<MAX_ITERATION; i++ )
                {
                    float3 pos = or + t*rayon;
                    float h = map(pos);
                     t += h;
                    if( h < 0.1 || t>MAX_DISTANCE )
                    {
                        break;
                    }
                 
                }
             

             float3 col = _BackgroundColor;
            if( t.x < MAX_DISTANCE)
            {
                float3 pos = or + t*rayon;
                float3 nor = calcNormal(pos);
                float dif = 1-clamp( dot(nor,float3(0.57703,57703,57703)), 0.0, 1.0 );
                float amb = 0.5 + 0.5*dot(nor,float3(1.0,1.0,0.0));
                col = float3(0.2,0.3,0.4)*amb + float3(0.8,0.7,0.5)*(dif*_BackgroundColor);
            }
            else
            {
               col = tex;
            }
                return float4(col,1);
            }
            ENDCG
        }
    }
}
