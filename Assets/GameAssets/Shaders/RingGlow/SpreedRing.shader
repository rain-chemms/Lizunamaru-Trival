Shader "UI/SpreadRing"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        
        [Header(Ring Settings)]
        [HDR] _Color ("光圈颜色", Color) = (1, 0.6, 0.2, 1)
        _Speed ("扩散速度", Range(0.1, 5.0)) = 1.0
        _RingWidth ("光圈宽度", Range(0.01, 0.3)) = 0.08
        _RingCount ("光圈数量", Range(1, 5)) = 1
        _FadeOut ("边缘衰减", Range(0.0, 1.0)) = 0.8
        
        [Header(Glow Settings)]
        _CenterGlow ("中心发光强度", Range(0.0, 2.0)) = 0.5
        _CenterGlowSize ("中心发光大小", Range(0.01, 0.5)) = 0.15
        
        [Header(Shape)]
        _AspectRatio ("宽高比修正", Range(0.0, 1.0)) = 1.0
        _Softness ("柔化程度", Range(0.001, 0.2)) = 0.03
        
        // --- UI 系统必需属性 ---
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
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
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
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
            Name "SpreadRing"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_UI_CLIP_RECT
            #pragma multi_compile _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
                float4 worldPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // --- 属性变量 ---
            sampler2D _MainTex;
            float4    _MainTex_ST;

            half4     _Color;
            half      _Speed;
            half      _RingWidth;
            half      _RingCount;
            half      _FadeOut;
            half      _CenterGlow;
            half      _CenterGlowSize;
            half      _AspectRatio;
            half      _Softness;

            float4    _ClipRect;

            // ==================================================
            //  顶点着色器
            // ==================================================
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.worldPos = o.vertex;
                o.uv       = TRANSFORM_TEX(v.uv, _MainTex);
                o.color    = v.color;
                return o;
            }

            // ==================================================
            //  片段着色器
            // ==================================================
            fixed4 frag(v2f i) : SV_Target
            {
                // ---------- UV 映射到以中心为原点 ----------
                float2 uv = i.uv - 0.5;  // 中心点 (0,0)

                // 宽高比修正：使光圈在不同尺寸 Image 上保持正圆
                float aspect = _AspectRatio;
                uv.x *= aspect;

                // 到中心的距离 [0, ~0.7]
                float dist = length(uv);

                // ---------- 时间驱动的扩散半径 ----------
                float time = _Time.y * _Speed;

                // ---------- 多光圈叠加 ----------
                half ringSum = 0;

                for (int k = 0; k < (int)_RingCount; k++)
                {
                    // 每个光圈有相位偏移，均匀分布
                    float phase = (float)k / (float)_RingCount;

                    // frac 产生 0→1 的循环动画
                    float ringPos = frac(time + phase);

                    // 光圈亮度：ringPos 越接近 dist 越亮
                    float diff = abs(dist - ringPos * 0.7); // 0.7 ≈ 对角线一半
                    half  ring = saturate(1.0 - diff / _RingWidth);

                    // 柔化边缘（smoothstep 风格）
                    ring = smoothstep(0.0, 1.0, ring);

                    // 边缘衰减：越靠外光圈越弱
                    float fadeFactor = 1.0 - saturate(ringPos * _FadeOut * 1.2);
                    ring *= fadeFactor;

                    ringSum += ring;
                }

                // ---------- 中心辉光 ----------
                half centerGlow = saturate(1.0 - dist / _CenterGlowSize);
                centerGlow = pow(centerGlow, 3.0); // 集中辉光
                centerGlow *= _CenterGlow;

                // 中心辉光也做呼吸效果
                float breathe = 0.5 + 0.5 * sin(time * 3.14159 * 2.0);
                centerGlow *= (0.7 + 0.3 * breathe);

                // ---------- 合成 ----------
                half alpha = saturate(ringSum + centerGlow);

                // 采样原始贴图颜色（支持 Image 上设置的 Sprite）
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // 最终颜色 = 光圈颜色 × alpha × 贴图 × 顶点颜色
                fixed4 finalColor;
                finalColor.rgb = _Color.rgb * alpha * texColor.rgb * i.color.rgb;
                finalColor.a   = alpha * texColor.a * i.color.a;

                // ---------- UI 裁剪矩形支持 ----------
                #ifdef UNITY_UI_CLIP_RECT
                    finalColor.a *= UnityGet2DClipping(i.worldPos.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                    clip(finalColor.a - 0.001);
                #endif

                return finalColor;
            }
            ENDCG
        }
    }
    Fallback "UI/Default"
}