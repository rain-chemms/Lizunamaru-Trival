Shader "Custom/URP/2DWaveBorder"
{
    Properties
    {
        [MainTexture] _MainTex ("主贴图", 2D) = "white" {}
        _Color ("颜色叠加", Color) = (1,1,1,1)

        
        [Space(8)]
        _WaveAmplitude ("波浪振幅(UV)", Range(0, 0.15)) = 0.03
        _WaveFrequency ("波浪频率", Range(0, 50)) = 10
        _WaveSpeed ("波浪速度", Range(0, 10)) = 2
        _WaveEdgeWidth ("边缘宽度(UV)", Range(0, 0.5)) = 0.2

        [Space(8)]
        _VertAmplitude ("顶点位移振幅", Range(0, 0.5)) = 0.05
        _VertFrequency ("顶点频率", Range(0, 50)) = 8
        _VertSpeed ("顶点速度", Range(0, 10)) = 1.5
        _VertEdgeWidth ("顶点边缘宽度", Range(0, 0.5)) = 0.25

        [Space(8)]
        _WaveOctaves ("叠加层数", Range(1, 4)) = 3
        _EdgeFalloff ("边缘衰减", Range(0.1, 5)) = 1.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "WavePass"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            // Unity 6 / URP 17 适配
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma target 3.5

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // -------- 结构体 --------
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float2 edgeFactor : TEXCOORD1;
                half4  color      : COLOR; 
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // -------- CBUFFER (SRP Batcher 兼容) --------
            CBUFFER_START(UnityPerMaterial)
                float4  _MainTex_ST;
                half4   _Color;
                float   _WaveAmplitude;
                float   _WaveFrequency;
                float   _WaveSpeed;
                float   _WaveEdgeWidth;
                float   _VertAmplitude;
                float   _VertFrequency;
                float   _VertSpeed;
                float   _VertEdgeWidth;
                uint    _WaveOctaves;
                float   _EdgeFalloff;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // -------- 工具函数 --------
            
            // 计算 UV 到矩形边缘的最短距离
            float EdgeDistance(float2 uv)
            {
                float2 d = min(uv, 1.0 - uv);
                return min(d.x, d.y);
            }

            // 计算边缘权重 (越靠近边缘越接近 1)
            float EdgeWeight(float dist, float width, float falloff)
            {
                float w = saturate(1.0 - dist / max(width, 0.0001));
                return pow(w, falloff);
            }

            // 多层正弦波叠加 (FBM)
            float MultiWave(float coord, float time, float freq, float speed, uint octaves)
            {
                float result = 0.0;
                float amp = 1.0;
                float totalAmp = 0.0;

                for (uint i = 0u; i < octaves; i++)
                {
                    result += amp * sin(coord * freq + time * speed + (float)i * 1.7);
                    totalAmp += amp;
                    freq *= 2.1;
                    speed *= 0.8;
                    amp *= 0.5;
                }
                return result / max(totalAmp, 0.0001);
            }

            // -------- 顶点着色器 --------
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                float2 uv = IN.uv;
                float time = _Time.y;

                // 1. 计算顶点边缘权重
                float dist = EdgeDistance(uv);
                float edgeW = EdgeWeight(dist, _VertEdgeWidth, _EdgeFalloff);

                // 2. 计算顶点 Z 轴位移
                float angle = atan2(uv.y - 0.5, uv.x - 0.5);
                float wave = MultiWave(angle * 2.0, time, _VertFrequency, _VertSpeed, _WaveOctaves);

                float3 displacedPos = IN.positionOS.xyz;
                displacedPos.z += wave * _VertAmplitude * edgeW;

                // 3. 空间变换 (Unity 6 / URP 17 必须传入 float4，w=1.0 表示位置点)
                VertexPositionInputs vertexInput = GetVertexPositionInputs(float4(displacedPos, 1.0));
                OUT.positionCS = vertexInput.positionCS;

                // 4. 传递数据到片元
                OUT.uv = TRANSFORM_TEX(uv, _MainTex);
                OUT.edgeFactor.x = EdgeWeight(dist, _WaveEdgeWidth, _EdgeFalloff);
                OUT.edgeFactor.y = angle;
                OUT.color = IN.color;

                return OUT;
            }

            // -------- 片元着色器 --------
            half4 frag(Varyings IN) : SV_Target
            {
                float time = _Time.y;
                float2 uv = IN.uv;
                float edgeW = IN.edgeFactor.x;
                float angle = IN.edgeFactor.y;

                // 1. 计算片元 UV 扰动
                float waveX = MultiWave(uv.y * 3.0 + angle, time,
                                        _WaveFrequency, _WaveSpeed, _WaveOctaves);
                float waveY = MultiWave(uv.x * 3.0 + angle, time,
                                        _WaveFrequency * 1.1, _WaveSpeed * 0.9, _WaveOctaves);

                float2 distortion = float2(waveX, waveY) * _WaveAmplitude * edgeW;
                float2 distortedUV = uv + distortion;

                // 2. 采样并输出
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);
                texColor *= _Color * IN.color;

                return texColor;
            }

            ENDHLSL
        }
    }
    
    FallBack "Hidden/InternalErrorShader"
}