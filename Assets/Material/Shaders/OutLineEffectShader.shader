Shader "Hidden/BOHU/OutLine"
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


            float3 _Color;
            float _DistanceModulation;
            float _Fresnel;
            float _GrazingAngleMaskPower;
            float _DepthThreshold;
            float _NormalThreshold;
            sampler2D _CameraDepthTexture;
            sampler2D _CameraDepthNormalsTexture;
            sampler2D _MainTex;
            float _FarPlaneDepth;
            float _NearClipPlaneDepth;
            float _Scale;
            float3 _CameraDirection;
            float _DepthNormalThresholdScale;
            static const int SobelX[3][3] = {{1, 0, -1},{2, 0, -2},{1, 0, -1}};
            static const int SobelY[3][3] = {{1, 2, 1},{0, 0, 0},{-1, -2, -1}};
            static const int RobertX[2][2] = {{1, 0},{0, -1 },};
            static const int RobertY[2][2] = {{0, 1},{-1, 0},};
            float4 _MainTex_TexelSize;
            float _TextureThreshold;
            float4x4 _ClipToView;
            float2 pixelSize;
            
            float4 invLerp(float4 A, float4 B, float4 T)
            {
                return (T - A)/(B - A);
            }

            
      
            float4 getRobertsCross(sampler2D sampl, float2 pos)
            {

                float4 Gx  = (0,0,0);
                float4 Gy = (0,0,0);
                
                for (int i = 0; i < 2;i++)
                {
                     for (int j = 0; j < 2;j++)
                    {
                        Gx += tex2D(sampl, pos + float2(i*pixelSize.x,j*pixelSize.y))* RobertX[i][j];
                        Gy +=tex2D(sampl, pos + float2(i*pixelSize.x,j*pixelSize.y)) * RobertY[i][j];
                    }
                }
                
                 return  abs(Gx) + abs(Gy);
            }
            
            float4 getSobel(sampler2D sampl,float2 pos)
            {
                   float4 Gx;
                float4 Gy;
                
                for (int i = 0; i < 3;i++)
                {
                    for (int j = 0; j < 3;j++)
                    {
                        float2 texcoord =  pos + float2(i*pixelSize.x,j*pixelSize.y);
                        Gx += tex2D(sampl,texcoord)* SobelX[i][j];
                        Gy += tex2D(sampl,texcoord) * SobelY[i][j];
                    }
                }
                
                 return  abs(Gx) + abs(Gy);
            }


            float3 getSobelNormal(float2 pos)
            {
                float3 Gx;
                float3 Gy;
                
                for (int i = 0; i < 3;i++)
                {
                    for (int j = 0; j < 3;j++)
                    {
                        
                        float2 texcoord =  pos + float2(i*pixelSize.x,j*pixelSize.y);
                        float4 Dnormals = (tex2D(_CameraDepthNormalsTexture,texcoord));
                        float depth = (tex2D(_CameraDepthTexture   , texcoord).r);
                        float3 normal;
                        DecodeDepthNormal(Dnormals,depth,normal);
                        normal = (normal+1)*0.5;
                        
                        Gx +=Dnormals* SobelX[i][j];
                        Gy += Dnormals * SobelY[i][j];
                    }
                }
                
                 return  abs(Gx) + abs(Gy);
            }
            
            
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
                o.viewSpaceDir = mul(_ClipToView, o.vertex).xyz;
                return o;
            }

            
            float hashOld12(float2 p)
            {
                // Two typical hashes...
	            return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
                
                // This one is better, but it still stretches out quite quickly...
                // But it's really quite bad on my Mac(!)
                //return fract(sin(dot(p, vec2(1.0,113.0)))*43758.5453123);

            }

            
            fixed4 frag (v2f i) : SV_Target
            {
                
                float2 uv = i.uv;
                
                pixelSize = float2(_MainTex_TexelSize.x*_Scale,_MainTex_TexelSize.y*_Scale);
                float depth = (tex2D(_CameraDepthTexture   , uv).r);
                float3 color = (tex2D(_MainTex   , uv));
                float depthEdgeD = depth * _DepthThreshold;
                float depthEdgeN = Linear01Depth(depth);
                depth = clamp(depth,0,_NearClipPlaneDepth);
                /*   
                float3 normal;
                DecodeDepthNormal(Dnormals,depth_normal,normal);
                */


                float4 Dnormals = (tex2D(_CameraDepthNormalsTexture,uv));
                float d = (tex2D(_CameraDepthTexture   , uv).r);
                float3 normal;
                DecodeDepthNormal(Dnormals,depth,normal);
             
                float VdotN = saturate( 1-dot(normal,-i.viewSpaceDir)) * _DepthNormalThresholdScale + 1;
                
;
                float2 waggyUv = uv  /*+  float2(cos(uv.y*100 )*(0.0005),cos(uv.x*100)*(0.0005))*/ ;
                
                float2 edgeDepth = getRobertsCross(_CameraDepthTexture,waggyUv).rg;
                float edgeD =  max(edgeDepth.x, edgeDepth.y);
                edgeD = edgeD*VdotN > depthEdgeD;
                
                float3 edgeNormal =  getSobelNormal(waggyUv).xyz;
                float edgeN = max(max(edgeNormal.x, edgeNormal.y), edgeNormal.z);
                edgeN = edgeN > (_NormalThreshold);

                
                float3 edgeTexture  = getRobertsCross(_MainTex,waggyUv).rgb;
                float edgeT =  max(max(edgeTexture.x, edgeTexture.y), edgeTexture.z);
                edgeT = edgeT> (_TextureThreshold);
                
                
                float edge =  (max(max( edgeN,edgeD),edgeT));
                float3 output = lerp(color ,_Color,edge.x);
                //float3 output = float3(edgeD.x,edgeD.x==0 && edgeN.x ,edgeN.x==0 && edgeT.x );
                
                return float4(output,1);
            }
            ENDCG
        }
    }
}
