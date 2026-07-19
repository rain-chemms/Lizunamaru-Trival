Shader "Custom/2D/Aurora_Physical_URP_Unlit"
{
    Properties
    {
        // === 物理发光剖面 ===
        [HDR] _ColorGreen ("Oxygen Green (100km)", Color) = (0.2, 1.0, 0.4, 1)
        [HDR] _ColorBluePurple ("Nitrogen Blue/Purple (150km)", Color) = (0.3, 0.2, 0.9, 1)
        [HDR] _ColorRed ("High-Alt Oxygen Red (200km+)", Color) = (0.9, 0.15, 0.1, 1)
        
        _GreenPeak ("Green Peak Altitude", Range(0.0, 0.4)) = 0.15
        _BluePeak ("Blue Peak Altitude", Range(0.2, 0.7)) = 0.45
        _RedPeak ("Red Peak Altitude", Range(0.6, 1.0)) = 0.85
        
        _BottomSharpness ("Bottom Edge Sharpness", Range(2, 20)) = 12.0
        _TopDiffusion ("Top Edge Diffusion", Range(0.5, 5)) = 2.5
        
        // === 磁场与粒子动力学 ===
        _FieldAlignStrength ("Magnetic Field Alignment", Range(0, 2)) = 1.2
        _CurtainFoldFreq ("Curtain Fold Frequency", Range(1, 10)) = 4.0
        _CurtainFoldAmp ("Curtain Fold Amplitude", Range(0, 0.3)) = 0.08
        _DriftSpeed ("Solar Wind Drift Speed", Range(0, 1)) = 0.15
        _RayDensity ("Vertical Ray Density", Range(5, 50)) = 25.0
        _RayContrast ("Ray Contrast", Range(0, 1)) = 0.6
        
        // === 全局 ===
        _MasterIntensity ("Master Intensity", Range(0, 3)) = 1.5
        _AlphaPower ("Overall Softness", Range(0.5, 3)) = 1.2
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
            Name "PhysicalAuroraPass"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _ColorGreen; half4 _ColorBluePurple; half4 _ColorRed;
                float _GreenPeak; float _BluePeak; float _RedPeak;
                float _BottomSharpness; float _TopDiffusion;
                float _FieldAlignStrength;
                float _CurtainFoldFreq; float _CurtainFoldAmp;
                float _DriftSpeed;
                float _RayDensity; float _RayContrast;
                float _MasterIntensity; float _AlphaPower;
            CBUFFER_END

            // ==================== 物理噪声系统 ====================
            inline float Hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            inline float ValueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                float a = Hash(i);
                float b = Hash(i + float2(1, 0));
                float c = Hash(i + float2(0, 1));
                float d = Hash(i + float2(1, 1));
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            // 各向异性 FBM：垂直方向拉伸，模拟磁力线约束
            inline float AnisotropicFBM(float2 p, float fieldAlign)
            {
                float v = 0.0;
                float amp = 0.5;
                // 垂直频率远低于水平频率 → 产生竖向条纹
                float2 freqScale = float2(1.0, max(0.1, 1.0 - fieldAlign * 0.4));
                
                for (int i = 0; i < 4; i++)
                {
                    v += amp * ValueNoise(p * freqScale);
                    p *= float2(2.0, 1.8); // 非均匀倍增保持各向异性
                    amp *= 0.5;
                }
                return v;
            }

            // ==================== 结构体 ====================
            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            // ==================== 片元着色器 ====================
            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                float time = _Time.y * _DriftSpeed;
                
                // ★ 1. 窗帘褶皱模型（Curtain Fold）
                // 极光本质是折叠的发光帘幕，水平位置决定帘幕的"深度"
                float foldPhase = uv.x * _CurtainFoldFreq + time * 0.5;
                float foldNoise = AnisotropicFBM(float2(foldPhase, time * 0.2), _FieldAlignStrength);
                float curtainDepth = sin(foldPhase * 6.2831) * _CurtainFoldAmp + (foldNoise - 0.5) * 0.05;
                
                // 将窗帘深度映射为可见度（面向观察者的褶皱更亮）
                float curtainVisibility = saturate(0.5 + curtainDepth * 8.0);
                
                // ★ 2. 垂直射线结构（Field-Aligned Rays）
                float rayNoise = AnisotropicFBM(uv * float2(_RayDensity, 2.0) + float2(time * 0.3, 0), _FieldAlignStrength);
                float rays = lerp(1.0, rayNoise, _RayContrast);
                
                // ★ 3. 物理发光剖面（Altitude Emission Profile）
                // 使用高斯分布模拟每种气体的发光峰值高度
                float alt = uv.y;
                float greenEmit  = exp(-pow((alt - _GreenPeak) * 12.0, 2.0));
                float blueEmit   = exp(-pow((alt - _BluePeak) * 8.0, 2.0));
                float redEmit    = exp(-pow((alt - _RedPeak) * 6.0, 2.0));
                
                // 归一化混合权重
                float totalEmit = greenEmit + blueEmit + redEmit + 0.001;
                half3 emissionColor = (
                    _ColorGreen.rgb * greenEmit + 
                    _ColorBluePurple.rgb * blueEmit + 
                    _ColorRed.rgb * redEmit
                ) / totalEmit;
                
                // 总发光强度（决定该像素是否"有光"）
                float emissionStrength = saturate(totalEmit * 1.5);
                
                // ★ 4. 非对称边缘（Asymmetric Edge Profile）
                // 底部锐利（粒子沉降边界），顶部弥散（大气稀薄）
                float bottomEdge = smoothstep(0.0, 0.15 / _BottomSharpness, alt);
                float topEdge = smoothstep(1.0, 1.0 - 0.3 * _TopDiffusion, alt);
                float verticalProfile = bottomEdge * topEdge;
                
                // ★ 5. 组合所有物理因子
                float intensity = curtainVisibility * rays * emissionStrength * verticalProfile;
                intensity = pow(saturate(intensity), _AlphaPower) * _MasterIntensity;
                
                // 预乘 Alpha 输出
                half3 finalColor = emissionColor * intensity;
                half finalAlpha = saturate(intensity);
                
                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
}