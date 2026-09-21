Shader "UI/RoundedRectFlowLight_Dissolve"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        
        // ========== 圆角控制 ==========
        _Radius        ("圆角半径 (0~0.5)", Range(0, 0.5)) = 0.1
        
        // ========== 边框控制 ==========
        _BorderWidth   ("边框宽度", Range(0, 0.1)) = 0.01
        _BorderColor   ("边框基础颜色", Color) = (1, 1, 1, 0.3)
        
        // ========== 流光控制 ==========
        _FlowColor     ("流光颜色", Color) = (0, 0.8, 1, 1)
        _FlowWidth     ("流光宽度", Range(0, 0.5)) = 0.08
        _FlowSpeed     ("流光速度", Range(-5, 5)) = 1.0
        _FlowIntensity ("流光强度", Range(0, 5)) = 2.0
        _FlowCount     ("流光数量(1或2)", Range(1, 2)) = 1

        // ========== 外发光(仅内侧/贴边) ==========
        _GlowSpread    ("发光扩散范围", Range(0, 0.1)) = 0.02
        _GlowIntensity ("发光强度", Range(0, 3)) = 1.0

        // ========== 圆角外部控制 ==========
        _OutsideAlpha  ("圆角外透明度", Range(0, 1)) = 0.0
        _OutsideColor  ("圆角外颜色", Color) = (0, 0, 0, 1)

        // ========== 🔥 HDR 火焰溶解控制 ==========
        _NoiseTex       ("溶解噪声贴图", 2D) = "white" {}
        _Threshold      ("溶解进度", Range(-0.1, 1.1)) = 0
        _EdgeWidth      ("火焰边缘宽度", Range(0, 0.5)) = 0.12
        [HDR] _FireColorCore  ("火焰核心 (白/黄)", Color) = (3.0, 2.5, 1.0, 1)
        [HDR] _FireColorMid   ("火焰中层 (橙)",     Color) = (2.5, 0.8, 0.0, 1)
        [HDR] _FireColorOuter ("火焰外层 (红)",     Color) = (1.8, 0.1, 0.0, 1)
        _FireFresnelPow ("火焰集中度", Range(0.5, 4)) = 1.5

        // ========== UI Stencil 支持 ==========
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil     ("Stencil ID", Float) = 0
        _StencilOp   ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask  ("Stencil Read Mask", Float) = 255
        _ColorMask   ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
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
            Name "ROUNDED_RECT_FLOW_DISSOLVE"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

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

            // ====== 原材质变量 ======
            sampler2D _MainTex;
            float4    _MainTex_ST;
            float  _Radius;
            float  _BorderWidth;
            float4 _BorderColor;
            float4 _FlowColor;
            float  _FlowWidth;
            float  _FlowSpeed;
            float  _FlowIntensity;
            float  _FlowCount;
            float  _GlowSpread;
            float  _GlowIntensity;
            float  _OutsideAlpha;
            float4 _OutsideColor;

            // ====== 溶解变量 ======
            sampler2D _NoiseTex;
            float4    _NoiseTex_ST;
            float  _Threshold;
            float  _EdgeWidth;
            half3  _FireColorCore;
            half3  _FireColorMid;
            half3  _FireColorOuter;
            float  _FireFresnelPow;

            float4 _ClipRect;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.worldPos = v.vertex;
                o.uv       = TRANSFORM_TEX(v.uv, _MainTex);
                o.color    = v.color;
                return o;
            }

            // 圆角矩形 SDF
            float sdRoundedRect(float2 p, float2 b, float r)
            {
                float2 q = abs(p) - b + float2(r, r);
                return min(max(q.x, q.y), 0.0)
                     + length(max(q, float2(0.0, 0.0))) - r;
            }

            // 周长参数化
            float perimeterParam(float2 p, float2 b, float r)
            {
                float2 q = abs(p);
                float2 cornerCenter = b - float2(r, r);
                float2 d = q - cornerCenter;
                float angle = atan2(d.y, d.x); 
                float t = angle / (2.0 * 3.14159265) + 0.5; 
                return t;
            }

            // ★ 三段式火焰颜色采样
            half3 SampleFireColor(float t)
            {
                half3 col;
                if (t < 0.5)
                {
                    float localT = t * 2.0;
                    col = lerp(_FireColorOuter, _FireColorMid, localT);
                }
                else
                {
                    float localT = (t - 0.5) * 2.0;
                    col = lerp(_FireColorMid, _FireColorCore, localT);
                }
                return col;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 p = uv - float2(0.5, 0.5);  
                float2 sdfB = float2(0.5, 0.5);
                
                // 计算 SDF 距离
                float dist = sdRoundedRect(p, sdfB, _Radius);

                // ==================== 1. 原材质：严格的内外分区 ====================
                float innerMask = 1.0 - smoothstep(-0.002, 0.002, dist);
                float outerMask = 1.0 - innerMask;

                // ==================== 2. 原材质：内部效果 ====================
                float borderMask = 0.0;
                if (_BorderWidth > 0.0)
                {
                    float bInner = smoothstep(-_BorderWidth - 0.001, -_BorderWidth + 0.001, dist);
                    float bOuter = innerMask;
                    borderMask = bInner * bOuter;
                }

                float t_param = perimeterParam(p, sdfB, _Radius);
                float time = _Time.y * _FlowSpeed;
                float flowT1 = frac(t_param + time);
                float flow1 = 0.0;
                if (_FlowWidth > 0.0)
                {
                    float halfW = _FlowWidth * 0.5;
                    flow1 = smoothstep(0.0, halfW, flowT1) * smoothstep(_FlowWidth, halfW, flowT1);
                }
                float flow2 = 0.0;
                if (_FlowCount >= 2 && _FlowWidth > 0.0)
                {
                    float flowT2 = frac(t_param - time + 0.5); 
                    float halfW = _FlowWidth * 0.5;
                    flow2 = smoothstep(0.0, halfW, flowT2) * smoothstep(_FlowWidth, halfW, flowT2);
                }
                float totalFlow = max(flow1, flow2);
                float flowMask = totalFlow * borderMask;

                float glowMask = 0.0;
                if (_GlowSpread > 0.0)
                {
                    float innerGlow = exp(dist / max(_GlowSpread, 0.001));
                    glowMask = innerGlow * _GlowIntensity * innerMask;
                    glowMask *= (1.0 + totalFlow * _FlowIntensity);
                }

                // ==================== 3. 原材质：外部效果 ====================
                float4 outsideContrib = _OutsideColor * _OutsideAlpha * outerMask;

                // ==================== 4. 原材质：最终合成 (作为 mainColor) ====================
                fixed4 texColor = tex2D(_MainTex, uv) * i.color;
                float4 borderContrib = _BorderColor * borderMask;
                float4 flowContrib = _FlowColor * flowMask * _FlowIntensity;
                float4 glowContrib = _FlowColor * glowMask * 0.5;

                fixed4 mainColor = float4(0, 0, 0, 0);
                mainColor.rgb = texColor.rgb * innerMask 
                              + borderContrib.rgb * borderContrib.a 
                              + flowContrib.rgb * flowContrib.a 
                              + glowContrib.rgb * glowContrib.a;
                
                mainColor.a = max(texColor.a * innerMask, 
                              max(borderContrib.a * innerMask, 
                              max(flowContrib.a, glowContrib.a)));

                mainColor.rgb += outsideContrib.rgb;
                mainColor.a = max(mainColor.a, outsideContrib.a);

                // =====================================================
                // ★ 5. HDR 火焰溶解叠加 (在原有合成结果之上执行)
                // =====================================================
                
                // 如果原材质该像素已经完全透明，直接跳过溶解计算
                if (mainColor.a <= 0.001)
                    discard;

                float2 noiseUV = TRANSFORM_TEX(uv, _NoiseTex);
                float noiseValue = tex2D(_NoiseTex, noiseUV).r;

                float diff = noiseValue - _Threshold;
                float edgeT = saturate(diff / _EdgeWidth);
                float fireT = pow(edgeT, _FireFresnelPow);
                half3 fireColor = SampleFireColor(fireT);

                fixed4 finalColor = mainColor;

                // 仅在溶解边缘区域内叠加火焰色（加法混合保持HDR）
                if (diff > 0 && diff < _EdgeWidth)
                {
                    float fireIntensity = sin(edgeT * 3.14159);
                    fireIntensity = pow(fireIntensity, 0.8);
                    finalColor.rgb += fireColor * fireIntensity;
                }

                // 溶解区域透明度过渡
                finalColor.a *= smoothstep(0, _EdgeWidth * 0.3, diff);

                // ==================== 6. UI 标准裁剪 ====================
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