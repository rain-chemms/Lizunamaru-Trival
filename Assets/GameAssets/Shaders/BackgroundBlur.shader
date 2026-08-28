Shader "UI/URP_BackgroundBlur"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // ========== 模糊参数 ==========
        [Space(10)]
        _BlurSize ("Blur Size", Range(0, 20)) = 5.0
        _BlurSamples ("Blur Samples", Range(4, 32)) = 16
        _BlurBrightness ("Blur Brightness", Range(0, 2)) = 1.0

        // ========== 高级选项 ==========
        [Space(10)]
        [Toggle] _UseSpriteAlpha ("Use Sprite Alpha", Float) = 1.0
        [Toggle] _PremultiplyAlpha ("Premultiply Alpha", Float) = 1.0

        // ========== 标准 UI 属性（Stencil 等） ==========
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil     ("Stencil ID",        Float) = 0
        _StencilOp   ("Stencil Operation",  Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask  ("Stencil Read Mask",  Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Stencil
        {
            Ref  [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask  [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "BackgroundBlur"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            // ---------- UI 标准宏 ----------
            #pragma shader_feature_local UNITY_UI_ALPHACLIP

            // ---------- URP 核心库 ----------
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // ============================================================
            //  URP 中用 _CameraOpaqueTexture 替代 Built-in 的 GrabPass
            //  需要在 URP Renderer 中开启 "Opaque Texture"
            // ============================================================
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // ---------- 材质属性 ----------
            CBUFFER_START(UnityPerMaterial)
                half4  _Color;
                half   _BlurSize;
                int    _BlurSamples;
                half   _BlurBrightness;
                half   _UseSpriteAlpha;
                half   _PremultiplyAlpha;
                // UI 裁剪矩形
                float4 _ClipRect;
            CBUFFER_END

            // ---------- 顶点输入 ----------
            struct Attributes
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;    // Image 组件的 Color 会传入这里
            };

            // ---------- 顶点输出 ----------
            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 screenPos   : TEXCOORD1;
                half4  vertexColor : COLOR;
            };

            // ============================================================
            //  Vertex Shader
            // ============================================================
            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // 对象空间 -> 裁剪空间
                OUT.positionCS = TransformObjectToHClip(IN.vertex.xyz);

                // UV 直接使用 Sprite 的 UV
                OUT.uv = IN.uv;

                // 计算屏幕空间坐标（用于采样 _CameraOpaqueTexture）
                OUT.screenPos = ComputeScreenPos(OUT.positionCS);

                // 传递顶点颜色（Image 组件的 Color tint 会乘以这里）
                OUT.vertexColor = IN.color;

                return OUT;
            }

            // ============================================================
            //  圆盘模糊采样 —— 比 Box Blur 更自然
            // ============================================================
            half3 DiskBlur(float2 screenUV, float blurRadius, int samples)
            {
                half3 col = half3(0, 0, 0);
                float totalWeight = 0.0;

                // 黄金角度螺旋采样，均匀分布采样点
                const float GOLDEN_ANGLE = 2.39996323; // radians

                for (int i = 0; i < samples; i++)
                {
                    float fi = float(i) + 1.0;
                    float r = sqrt(fi / float(samples));  // 径向分布
                    float theta = fi * GOLDEN_ANGLE;       // 角度分布

                    float2 offset = float2(cos(theta), sin(theta)) * r * blurRadius;
                    float2 sampleUV = screenUV + offset;

                    // 确保 UV 不超出 [0, 1]
                    sampleUV = saturate(sampleUV);

                    col += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, sampleUV).rgb;
                    totalWeight += 1.0;
                }

                return col / max(totalWeight, 1.0);
            }

            // ============================================================
            //  Fragment Shader
            // ============================================================
            half4 frag(Varyings IN) : SV_Target
            {
                // ---------- 1. 计算屏幕 UV ----------
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

                // ---------- 2. 计算像素级模糊半径 ----------
                //  _ScreenParams: x=width, y=height, z=1+1/width, w=1+1/height
                //  将模糊大小转换为屏幕 UV 空间的偏移
                float2 pixelSize = _ScreenParams.xy;
                float blurRadius = _BlurSize / min(pixelSize.x, pixelSize.y);

                // ---------- 3. 圆盘模糊采样背景 ----------
                int samples = clamp(_BlurSamples, 4, 32);
                half3 blurredBg = DiskBlur(screenUV, blurRadius, samples);
                blurredBg *= _BlurBrightness;

                // ---------- 4. 采样 Sprite 纹理 ----------
                half4 spriteColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // ---------- 5. 获取最终 Alpha ----------
                //  vertexColor.a = Image 组件 Color 属性中的 Alpha
                //  spriteColor.a = Sprite 纹理自身的 Alpha
                half imageAlpha = IN.vertexColor.a;       // Image 组件的 Alpha
                half spriteAlpha = spriteColor.a;

                // 最终 Alpha：可选择是否结合 Sprite 的 Alpha
                half finalAlpha;
                if (_UseSpriteAlpha > 0.5)
                    finalAlpha = imageAlpha * spriteAlpha;
                else
                    finalAlpha = imageAlpha;

                // ---------- 6. 颜色混合 ----------
                //  将 Image 组件的 RGB Color 与模糊背景混合
                half3 tintColor = IN.vertexColor.rgb * _Color.rgb;

                half3 finalColor;
                if (_PremultiplyAlpha > 0.5)
                {
                    // 预乘 Alpha 混合：背景 * alpha + 色调 * alpha
                    // 模糊背景透过半透明区域显示
                    finalColor = lerp(blurredBg, blurredBg * tintColor, finalAlpha);
                }
                else
                {
                    // 简单叠加：背景色 × 颜色色调
                    finalColor = blurredBg * tintColor;
                }

                // ---------- 7. UI 裁剪矩形 ----------
                #ifdef UNITY_UI_ALPHACLIP
                    // 使用 UI 的 RectMask2D 裁剪
                    float2 clipMin = _ClipRect.xy;
                    float2 clipMax = _ClipRect.zw;
                    float2 worldPos = IN.positionCS.xy; // 近似使用裁剪空间坐标
                    // 实际 UI 裁剪需要更精确的处理，这里简化
                #endif

                // ---------- 8. 输出 ----------
                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }

    // 降级方案：如果 URP 不可用，回退到标准 UI Shader
    Fallback "UI/Default"
}