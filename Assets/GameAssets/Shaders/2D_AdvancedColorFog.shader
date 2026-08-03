Shader "Custom/2D_AdvancedColorFog"
{
    Properties
    {
        _NoiseTex ("Noise Texture (Seamless)", 2D) = "white" {}
        _ColorRamp ("Color Ramp Texture", 2D) = "white" {}
        
        [Header(Flow Settings)]
        _Speed ("Base Flow Speed", Float) = 0.1
        _DetailSpeed ("Detail Flow Speed", Float) = 0.25
        _Tiling ("Base Tiling", Vector) = (1,1,0,0)
        _DetailTiling ("Detail Tiling", Vector) = (3,3,0,0)
        _DetailStrength ("Detail Noise Strength", Range(0, 1)) = 0.3
        
        [Header(Fog Appearance)]
        _Density ("Overall Density", Range(0, 3)) = 1.0
        _RampOffset ("Color Ramp Offset", Range(-1, 1)) = 0
        _RampScale ("Color Ramp Scale", Range(0.1, 5)) = 1.0
        
        [Toggle] _UseHeight ("Enable Height Falloff", Float) = 0
        _HeightScale ("Height Falloff Scale", Float) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+100" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes 
            { 
                //float4 positionOS : POSITION; 
                //float2 uv : TEXCOORD0; 
                float4 positionOS : POSITION; 
                float2 uv : TEXCOORD0; 
                float4 color : COLOR; //新增：接收UI传入的顶点色
            };
            
            struct Varyings 
            { 
                float4 positionCS : SV_POSITION; 
                float2 baseUV : TEXCOORD0; 
                float2 detailUV : TEXCOORD1;
                float screenY : TEXCOORD2;
                float4 vertexColor : COLOR; //新增：传递到片元阶段
            };
            
            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);
            TEXTURE2D(_ColorRamp); SAMPLER(sampler_ColorRamp);
            
            float _Speed;
            float _DetailSpeed;
            float4 _Tiling;
            float4 _DetailTiling;
            float _DetailStrength;
            float _Density;
            float _RampOffset;
            float _RampScale;
            float _UseHeight;
            float _HeightScale;
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                
                // 基础层UV：大尺度、慢速
                output.baseUV = input.uv * _Tiling.xy + float2(_Time.y * _Speed, _Time.y * _Speed * 0.3);
                
                // 细节层UV：小尺度、快速、反向移动增加混沌感
                output.detailUV = input.uv * _DetailTiling.xy + float2(-_Time.y * _DetailSpeed, _Time.y * _DetailSpeed * 0.7);
                
                output.screenY = input.uv.y;
                output.vertexColor = input.color; //传递顶点色
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // === 1. 双层噪声混合 ===
                half baseNoise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.baseUV).r;
                half detailNoise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.detailUV).r;
                
                // 将细节噪声作为偏移量叠加到基础噪声上（Domain Warping简化版）
                half combinedNoise = saturate(baseNoise + (detailNoise - 0.5) * _DetailStrength);
                
                // === 2. 应用密度与高度衰减 ===
                half fogAmount = combinedNoise * _Density;
                
                if (_UseHeight > 0.5)
                {
                    half heightFactor = saturate((1.0 - input.screenY) * _HeightScale);
                    fogAmount *= heightFactor;
                }
                
                fogAmount = saturate(fogAmount);
                
                // === 3. 多色Ramp采样 ===
                // 用噪声值驱动Ramp贴图的U坐标，实现颜色随浓度变化
                half rampUV = saturate((combinedNoise + _RampOffset) * _RampScale);
                half4 fogColor = SAMPLE_TEXTURE2D(_ColorRamp, sampler_ColorRamp, float2(rampUV, 0.5));
                // Alpha由最终雾浓度决定，RGB由Ramp决定
                // UI的tint color和alpha动画都通过vertexColor传递
                half4 finalColor = half4(fogColor.rgb * input.vertexColor.rgb, 
                              fogAmount * fogColor.a * input.vertexColor.a);
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}