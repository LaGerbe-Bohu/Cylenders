Shader "Unlit/CameraOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FarPlaneDepth ("Soft Particles Factor", float) = 1.0
        _TexelDistance ("Soft Particles Factor", range(0.0001,0.01)) = 1.0
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 screenuv : TEXCOORD1;
                 float4 projPos : TEXCOORD4;
            };


        
                        
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenuv = ComputeScreenPos(o.vertex);

                   o.projPos = ComputeScreenPos (o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }

            sampler2D _CameraDepthTexture   ;
            sampler2D _CameraDepthNormalsTexture    ;
            float _FarPlaneDepth;
            float _TexelDistance;
            float4 invLerp(float4 A, float4 B, float4 T)
            {
                return (T - A)/(B - A);
            }


            const float3x3 SobelX{
                _TexelDistance, 0, -_TexelDistance,
                2*_TexelDistance, 0, -2*_TexelDistance,
                _TexelDistance, 0, -_TexelDistance
            };


             const float3x3 SobelY{
                   _TexelDistance, 2*_TexelDistance, _TexelDistance,
                0, 0, 0,
                -_TexelDistance, -2*_TexelDistance, -_TexelDistance
            };
       
            float getRobertsCross(sampler2D sampl, float2 pos)
            {

                if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y > 1)
                {
                    return 0.0;
                }

     
                
                float y0 = tex2D(sampl, pos).r;
                float y1 = tex2D(sampl, pos + float2(_TexelDistance, _TexelDistance)).r;
                float y2 = tex2D(sampl, pos + float2(_TexelDistance, 0)).r;
                float y3 = tex2D(sampl, pos + float2(0, _TexelDistance)).r;

                float Gx = abs(y0 - y1);
                float Gy = abs(y2 - y3);

                return Gx+Gy;
            }
            
            float getSobel(sampler2D,float2 pos)
            {
                float Gx = 0.0;

                float uv = mul(SobelX,pos);

                
                Gx += samples[0] * SobelX[0]; // top left (factor +1)
                Gx += samples[2] * SobelX[2]; // top right (factor -1)
                Gx += samples[3] * SobelX[3]; // center left (factor +2)
                Gx += samples[4] * SobelX[4]; // center right (factor -2)
                Gx += samples[5] * SobelX[5]; // bottom left (factor +1)
                Gx += samples[7] * SobelX[7]; // bottom right (factor -1)

                float Gy = 0.0;
                Gy += samples[0] * SobelY[0]; // top left (factor +1)
                Gy += samples[1] * SobelY[1]; // top center (factor +2)
                Gy += samples[2] * SobelY[2]; // top right (factor +1)
                Gy += samples[5] * SobelY[5]; // bottom left (factor -1)
                Gy += samples[6] * SobelY[6]; // bottom center (factor -2)
                Gy += samples[7] * SobelY[7];
                
                return  0.0;   
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                
                float2 uv = i.screenuv.xy / i.screenuv.w;
                float depth = (tex2D(_CameraDepthTexture   , uv).r);
                float3 Dnormals = (tex2D(_CameraDepthNormalsTexture   , uv));
                float3 color = (tex2D(_MainTex   , uv));
                
                depth = LinearEyeDepth(depth);
                depth = 1-invLerp(0,_FarPlaneDepth,depth);

                
           


                float Outline =( getRobertsCross(_MainTex,uv).x + getRobertsCross(_CameraDepthTexture,uv).x   + getRobertsCross(_CameraDepthNormalsTexture,uv).x ) ;

                float3 output = lerp(color,float3(0,0,0),Outline.x);

                return float4( Outline.xxx,1);
            }
            ENDCG
        }
    }
}
