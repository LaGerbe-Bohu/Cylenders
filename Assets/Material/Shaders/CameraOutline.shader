Shader "Hidden/CameraOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FarPlaneDepth ("FarPlaneDepth", float) = 1.0
        _Scale ("Scale", float) = 1.0
        _DistanceModulation ("DistanceModulation", float) = 1.0
        _DepthThreshold ("DepthThreshold", float) = 1.0
        _NormalThreshold ("NormalThreshold", float) = 1.0
        _Color("EdgeColor",color)=(0,0,0)
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
            float _Scale;
            float4 invLerp(float4 A, float4 B, float4 T)
            {
                return (T - A)/(B - A);
            }


            static const int SobelX[3][3] = {
                {1, 0, -1},
                {2, 0, -2},
                {1, 0, -1}
            };


             static const int SobelY[3][3] = {
                {1, 2, 1},
                {0, 0, 0},
                {-1, -2, -1}
            };


            
            static const int RobertX[2][2] = {
                {1, 0},
                {0, -1 },
             
            };


             static const int RobertY[2][2] = {
                {0, 1},
                {-1, 0},
            };

            float4 _MainTex_TexelSize;
            float4 getRobertsCross(sampler2D sampl, float2 pos)
            {

                float4 Gx  = (0,0,0);
                float4 Gy = (0,0,0);
                
                for (int i = 0; i < 2;i++)
                {
                     for (int j = 0; j < 2;j++)
                    {
                        Gx += tex2D(sampl, pos + float2(i*_MainTex_TexelSize.x*_Scale,j*_MainTex_TexelSize.y*_Scale))* RobertX[i][j];
                        Gy +=tex2D(sampl, pos + float2(i*_MainTex_TexelSize.x*_Scale,j*_MainTex_TexelSize.y*_Scale)) * RobertY[i][j];
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
                        Gx += tex2D(sampl, pos + float2(i*_MainTex_TexelSize.x*_Scale,j*_MainTex_TexelSize.y*_Scale))* SobelX[i][j];
                        Gy +=tex2D(sampl, pos + float2(i*_MainTex_TexelSize.x*_Scale,j*_MainTex_TexelSize.y*_Scale)) * SobelY[i][j];
                    }
                }
                
                 return  abs(Gx) + abs(Gy);
            }
            float3 _Color;
            float _DistanceModulation;
            float _Fresnel;
            float _GrazingAngleMaskPower;
            float _DepthThreshold;
            float _NormalThreshold;
            fixed4 frag (v2f i) : SV_Target
            {
                
                float2 uv =i.screenuv.xy/i.screenuv.w;
                
                float depth = (tex2D(_CameraDepthTexture   , uv).r);
                float4 Dnormals = (tex2D(_CameraDepthNormalsTexture   , uv));
                float3 color = (tex2D(_MainTex   , uv));
                
                depth = (depth);
                depth = invLerp(0,_FarPlaneDepth,depth);
                
                float3 V = UNITY_MATRIX_IT_MV[2].xyz;
                
                float3 normal;
                
                depth = depth * _DepthThreshold;

                float trehsoldNormal = Dnormals * _NormalThreshold;
                
                //float Outline =( getRobertsCross(_MainTex,uv).x + getRobertsCross(_CameraDepthTexture,uv).x   + getRobertsCross(_CameraDepthNormalsTexture,uv).x ) ;
                float edgeDepth =   ( getSobel(_CameraDepthTexture,uv).r);
  
                edgeDepth = edgeDepth >depth  ? 1:0;
                
                float edgeNormal =   (  getSobel(_CameraDepthNormalsTexture,uv).r)+(  getSobel(_CameraDepthNormalsTexture,uv).g);
                edgeNormal = edgeNormal > (1-depth)*_NormalThreshold  ? 1:0;
                float edge =  edgeDepth + edgeNormal;
                
                float3 output = lerp(color ,_Color,edge.x);
				
                return float4(1,1,1,1);
            }
            ENDCG
        }
    }
}
