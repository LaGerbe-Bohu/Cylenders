Shader "Unlit/CameraOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FarPlaneDepth ("FarPlaneDepth", float) = 1.0
        _TexelDistance ("TexelDistance", range(0.0001,0.01)) = 1.0
        _DistanceModulation ("DistanceModulation", float) = 1.0
        _Fresnel ("Fresnel", float) = 1.0
        _GrazingAngleMaskPower("GrazingAngleMaskPower",float)=0
        _Color("Color",color)=(0,0,0)
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
            
            float3 getRobertsCross(sampler2D sampl, float2 pos)
            {

                if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y > 1)
                {
                    return 0.0;
                }

     
                
                float3 y0 = tex2D(sampl, pos);
                float3 y1 = tex2D(sampl, pos + float2(_TexelDistance, _TexelDistance));
                float3 y2 = tex2D(sampl, pos + float2(_TexelDistance, 0));
                float3 y3 = tex2D(sampl, pos + float2(0, _TexelDistance));

                float3 Gx = abs(y0 - y1);
                float3 Gy = abs(y2 - y3);

                return Gx+Gy;
            }
            
            float3 getSobel(sampler2D sampl,float2 pos)
            {

                
                float3 Gx;
                float3 Gy;
                
                for (int i = 0; i < 3;i++)
                {
                     for (int j = 0; j < 3;j++)
                    {
                        Gx += tex2D(sampl, pos + float2(i*_TexelDistance,j*_TexelDistance))* SobelX[i][j];
                        Gy +=tex2D(sampl, pos + float2(i*_TexelDistance,j*_TexelDistance)) * SobelY[i][j];
                    }
                }
                
                 return  abs(Gx) + abs(Gy);
            }
            float3 _Color;
            float _DistanceModulation;
            float _Fresnel;
            float _GrazingAngleMaskPower;
            fixed4 frag (v2f i) : SV_Target
            {
                
                float2 uv = i.screenuv.xy / i.screenuv.w;
                
                float depth = (tex2D(_CameraDepthTexture   , uv).r);
                float4 Dnormals = (tex2D(_CameraDepthNormalsTexture   , uv));
                float3 color = (tex2D(_MainTex   , uv));
                
                depth = LinearEyeDepth(depth);
                depth = 1-invLerp(0,_FarPlaneDepth,depth);
                
                float3 V = UNITY_MATRIX_IT_MV[2].xyz;
                
                float3 normal;
                DecodeDepthNormal(Dnormals, depth, normal);
             
                float fresnel = pow(1.0 - saturate(dot(normal, -V)), _Fresnel);
                
                //float Outline =( getRobertsCross(_MainTex,uv).x + getRobertsCross(_CameraDepthTexture,uv).x   + getRobertsCross(_CameraDepthNormalsTexture,uv).x ) ;
                float2 Outline =   1-( getSobel(_CameraDepthTexture,uv).r + (getSobel(_CameraDepthNormalsTexture,uv).x + getSobel(_MainTex,uv).x ));
                float3 output = smoothstep(.1,1,Outline.xxx);
                float3 c = lerp(float3(0,0,0),color,Outline.x);
                
                return float4(c,1);
            }
            ENDCG
        }
    }
}
