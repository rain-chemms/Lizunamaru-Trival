Shader "Custom/SpriteEdgeGlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        
        // [HDR] 标签让颜色在 Inspector 中显示 HDR 拾色器，允许强度大于 1
        [HDR] _GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
        
        _GlowWidth ("Glow Width (UV Scale)", Range(0, 0.1)) = 0.02
        _GlowFalloff ("Glow Falloff", Range(0.1, 10)) = 2.0
        _GlowIntensity ("Intensity Multiplier", Range(1, 20)) = 1.0
        
        // 内部填充控制 (1 = 原图内部也带一点发光底色, 0 = 仅边缘发光)
        _InnerFill ("Inner Fill", Range(0, 1)) = 0.0

        // --- UGUI 兼容属性 ---
        _Color ("Tint", Color) = (1,1,1,1)
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
            "Queue"="Transparent"
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
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

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
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            half4 _GlowColor;
            half _GlowWidth;
            half _GlowFalloff;
            half _GlowIntensity;
            half _InnerFill;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // 获取中心点原始颜色
                half4 centerCol = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                half centerAlpha = centerCol.a;

                // 8方向采样偏移检测边缘
                half2 uv = IN.texcoord;
                half w = _GlowWidth;
                
                half a1 = tex2D(_MainTex, uv + half2( w,  0)).a;
                half a2 = tex2D(_MainTex, uv + half2(-w,  0)).a;
                half a3 = tex2D(_MainTex, uv + half2( 0,  w)).a;
                half a4 = tex2D(_MainTex, uv + half2( 0, -w)).a;
                
                half a5 = tex2D(_MainTex, uv + half2( w,  w) * 0.707).a;
                half a6 = tex2D(_MainTex, uv + half2(-w,  w) * 0.707).a;
                half a7 = tex2D(_MainTex, uv + half2( w, -w) * 0.707).a;
                half a8 = tex2D(_MainTex, uv + half2(-w, -w) * 0.707).a;

                // 获取周围最大 Alpha 值
                half maxSurroundAlpha = max(max(max(a1, a2), max(a3, a4)), max(max(a5, a6), max(a7, a8)));
                
                // 计算边缘遮罩 (如果周围不透明，但中心透明或半透明，则为边缘)
                half edgeMask = saturate(maxSurroundAlpha - centerAlpha);
                
                // 计算内部填充遮罩
                half innerMask = centerAlpha * _InnerFill;
                
                // 合并遮罩并应用衰减曲线
                half finalMask = pow(saturate(edgeMask + innerMask), _GlowFalloff);

                #ifdef UNITY_UI_CLIP_RECT
                centerCol.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(centerCol.a - 0.001);
                #endif

                // 计算最终颜色 (原图 + HDR发光)
                // 注意：发光部分直接加到 RGB 上，不进行 Alpha 混合限制，以保留 HDR 大于 1 的值
                half3 glowRGB = _GlowColor.rgb * finalMask * _GlowIntensity;
                
                half4 finalColor = centerCol;
                finalColor.rgb += glowRGB; 
                
                // 确保发光区域的 Alpha 至少和原图一样，防止透明像素被 Bloom 裁剪
                finalColor.a = max(centerCol.a, finalMask * _GlowColor.a);

                return finalColor;
            }
        ENDCG
        }
    }
}