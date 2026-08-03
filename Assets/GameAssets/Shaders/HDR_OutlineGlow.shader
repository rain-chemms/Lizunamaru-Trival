Shader "Custom/UI/HDR_OutlineGlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Source Texture (Outline Mask)", 2D) = "white" {}
        
        // --- HDR 发光控制 ---
        [Header(HDR Glow Settings)]
        [HDR] _GlowColor ("HDR Glow Color", Color) = (0, 2, 2, 1)
        _GlowIntensity ("Glow Intensity Multiplier", Range(0, 20)) = 5.0
        _GlowSpread ("Glow Spread / Blur Radius", Range(0, 0.5)) = 0.08
        _GlowSmoothness ("Edge Softness", Range(0.001, 0.5)) = 0.15
        
        // --- 源纹理处理 ---
        [Header(Source Processing)]
        [Toggle] _UseAlphaAsMask ("Use Alpha as Outline Mask", Float) = 1
        _Threshold ("Outline Threshold", Range(0, 1)) = 0.01

        // UGUI Required
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        //呼吸频率处理
        _BlurPulseSpeed("Blur Pulse Speed",Float) = 1
        _BlurPulseAmplitude("Blur Pulse Amplitude",Float) = 1
        _LightHDRPulseSpeed("Light HDR Pulse Speed",Float) = 1
        _LightHDRPulseAmplitude("Light HDR Pulse Amplitude",Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent+1" // 确保渲染在普通UI之上
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
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
        
        // 【关键】改为加法混合，允许颜色值 > 1.0 触发 Bloom
        Blend SrcAlpha One
        ColorMask [_ColorMask]

        Pass
        {
            Name "HDR_OutlineGlowPass"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            half4 _GlowColor;
            float _GlowIntensity;
            float _GlowSpread;
            float _GlowSmoothness;
            float _UseAlphaAsMask;
            float _Threshold;

            float _BlurPulseSpeed;
            float _BlurPulseAmplitude;
            float _LightHDRPulseSpeed;
            float _LightHDRPulseAmplitude;


            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.worldPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            float GetOutlineMask(float2 uv)
            {
                half4 tex = tex2D(_MainTex, uv);
                if (_UseAlphaAsMask > 0.5)
                    return tex.a;
                else
                    return dot(tex.rgb, half3(0.299, 0.587, 0.114));
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 uv = i.texcoord;
                float centerMask = GetOutlineMask(uv);
                
                if (centerMask < _Threshold)
                    discard;

                //定义呼吸
                //float breathe = (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5);//定义呼吸机制
                float blurBreathe =  (1.0 + (sin(_Time.y * _BlurPulseSpeed) * 0.5 + 0.5) * _BlurPulseAmplitude);
                float lightHDRBreathe =  (1.0 + (sin(_Time.y * _BlurPulseSpeed) * 0.5 + 0.5) * _LightHDRPulseAmplitude);

                // 8方向扩散采样
                float glowAccum = 0.0;
                float spread = _GlowSpread * blurBreathe;
                glowAccum += GetOutlineMask(uv + float2( spread,  0));
                glowAccum += GetOutlineMask(uv + float2(-spread,  0));
                glowAccum += GetOutlineMask(uv + float2( 0,  spread));
                glowAccum += GetOutlineMask(uv + float2( 0, -spread));
                glowAccum += GetOutlineMask(uv + float2( spread,  spread) * 0.707);
                glowAccum += GetOutlineMask(uv + float2(-spread,  spread) * 0.707);
                glowAccum += GetOutlineMask(uv + float2( spread, -spread) * 0.707);
                glowAccum += GetOutlineMask(uv + float2(-spread, -spread) * 0.707);
                glowAccum /= 8.0;

                // 【关键】不使用 saturate，保留 >1 的HDR数值
                float finalGlow = smoothstep(0.0, _GlowSmoothness, max(centerMask, glowAccum));//应用呼吸强度
                
                // 基础发光 × HDR颜色 × 强度倍增
                half4 result = _GlowColor * finalGlow * _GlowIntensity * lightHDRBreathe;
                
                // Alpha仅用于UI裁剪判断，不参与颜色钳制
                result.a = finalGlow * i.color.a;

                #ifdef UNITY_UI_ALPHACLIP
                    clip(result.a - 0.001);
                #endif

                return result;
            }
            ENDCG
        }
    }
}