Shader "UI/RoundedRectFlowLight"
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

        // ========== UI Stencil 支持 ==========
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil     ("Stencil ID", Float) = 0
        _StencilOp   ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask  ("Stencil Read Mask", Float) = 255
        _ColorMask   ("Color Mask", Float) = 15
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
            Name "ROUNDED_RECT_FLOW"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
                float4 worldPos : TEXCOORD1;
            };

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

            v2f vert(appdata v)
            {
                v2f o;
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

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 p = uv - float2(0.5, 0.5);  
                float2 sdfB = float2(0.5, 0.5);
                
                // 计算 SDF 距离 (负数=内部, 正数=外部)
                float dist = sdRoundedRect(p, sdfB, _Radius);

                // ==================== 核心修改：严格的内外分区 ====================
                
                // 1. 内部遮罩 (带抗锯齿)
                float innerMask = 1.0 - smoothstep(-0.002, 0.002, dist);
                
                // 2. 外部遮罩 (带抗锯齿，与内部完美互补)
                float outerMask = 1.0 - innerMask;

                // ==================== 内部效果 (严格限制在 innerMask 内) ====================
                
                // 边框：仅在内部边缘
                float borderMask = 0.0;
                if (_BorderWidth > 0.0)
                {
                    float bInner = smoothstep(-_BorderWidth - 0.001, -_BorderWidth + 0.001, dist);
                    float bOuter = innerMask; // ⬅️ 关键：用 innerMask 替代原来的 smoothstep，杜绝外部溢出
                    borderMask = bInner * bOuter;
                }

                // 流光：仅在边框区域内
                float t = perimeterParam(p, sdfB, _Radius);
                float time = _Time.y * _FlowSpeed;
                float flowT1 = frac(t + time);
                float flow1 = 0.0;
                if (_FlowWidth > 0.0)
                {
                    float halfW = _FlowWidth * 0.5;
                    flow1 = smoothstep(0.0, halfW, flowT1) * smoothstep(_FlowWidth, halfW, flowT1);
                }
                float flow2 = 0.0;
                if (_FlowCount >= 2 && _FlowWidth > 0.0)
                {
                    float flowT2 = frac(t - time + 0.5); 
                    float halfW = _FlowWidth * 0.5;
                    flow2 = smoothstep(0.0, halfW, flowT2) * smoothstep(_FlowWidth, halfW, flowT2);
                }
                float totalFlow = max(flow1, flow2);
                float flowMask = totalFlow * borderMask; // 流光已经被 borderMask 限制在内部

                // 发光：改为【内侧贴边辉光】，不再向外部漫射
                float glowMask = 0.0;
                if (_GlowSpread > 0.0)
                {
                    // 只在 dist < 0 (内部) 且靠近边缘的地方发光
                    // exp(dist / spread) 在 dist=0 时为1，dist越负衰减越快
                    float innerGlow = exp(dist / max(_GlowSpread, 0.001));
                    glowMask = innerGlow * _GlowIntensity * innerMask; // ⬅️ 关键：乘以 innerMask
                    
                    // 流光经过的地方增强辉光
                    glowMask *= (1.0 + totalFlow * _FlowIntensity);
                }

                // ==================== 外部效果 (纯净，不受光照影响) ====================
                float4 outsideContrib = _OutsideColor * _OutsideAlpha * outerMask;

                // ==================== 最终合成 ====================
                fixed4 texColor = tex2D(_MainTex, uv) * i.color;
                float4 borderContrib = _BorderColor * borderMask;
                float4 flowContrib = _FlowColor * flowMask * _FlowIntensity;
                float4 glowContrib = _FlowColor * glowMask * 0.5;

                // 内部合成
                float4 finalColor = float4(0, 0, 0, 0);
                finalColor.rgb = texColor.rgb * innerMask 
                               + borderContrib.rgb * borderContrib.a 
                               + flowContrib.rgb * flowContrib.a 
                               + glowContrib.rgb * glowContrib.a;
                
                finalColor.a = max(texColor.a * innerMask, 
                               max(borderContrib.a * innerMask, 
                               max(flowContrib.a, glowContrib.a)));

                // 外部叠加 (纯净底色，无光照混合)
                finalColor.rgb += outsideContrib.rgb;
                finalColor.a = max(finalColor.a, outsideContrib.a);

                // 智能裁切
                if (_OutsideAlpha <= 0.001 && glowContrib.a <= 0.001)
                {
                    clip(finalColor.a - 0.001);
                }

                return finalColor;
            }
            ENDCG
        }
    }
    Fallback "UI/Default"
}