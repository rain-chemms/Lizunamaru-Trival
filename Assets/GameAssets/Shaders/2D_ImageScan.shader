Shader "2D/2D_ImageScan"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1, 1, 1, 1)

        [Toggle(_USE_TIME)] _UseTime ("Auto Animate (Use Time)", Float) = 1
        _ScanProgress ("Manual Scan Progress (0~1)", Range(-0.5, 1.5)) = 0.5
        _ScanSpeed ("Scan Speed", Float) = 1.0
        _ScanWidth ("Scan Line Width", Range(0.001, 0.3)) = 0.05
        _ScanColor ("Scan Line Color", Color) = (0.0, 1.0, 1.0, 1.0)
        _ScanIntensity ("Scan Line Intensity", Float) = 2.5
        _ScanAngle ("Scan Line Angle (Degrees)", Range(-90, 90)) = 15
        _ScanDirection ("Scan Direction (0=LTR, 1=RTL)", Range(0, 1)) = 0
        _ScanRepeat ("Repeat Interval (sec, 0=no repeat)", Float) = 3.0

        _GlowWidth ("Glow Trail Width", Range(0, 1.0)) = 0.25
        _GlowColor ("Glow Trail Color", Color) = (0.0, 0.5, 1.0, 1.0)
        _GlowIntensity ("Glow Intensity", Float) = 0.6

        [Toggle] _EdgeHighlight ("Enable Edge Highlight", Float) = 1
        _EdgeColor ("Edge Color", Color) = (1, 1, 1, 1)
        _EdgeWidth ("Edge Width", Range(0, 0.1)) = 0.02
        _EdgeIntensity ("Edge Intensity", Float) = 1.0

        // ---- UI Stencil Support ----
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent+100"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Pass
        {
            Name "ScanEffect"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature_local _USE_TIME

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            // --- Texture & Color ---
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            half4 _Color;

            // --- Scan ---
            half  _ScanProgress;
            half  _ScanSpeed;
            half  _ScanWidth;
            half4 _ScanColor;
            half  _ScanIntensity;
            half  _ScanAngle;
            half  _ScanDirection;
            half  _ScanRepeat;

            // --- Glow ---
            half  _GlowWidth;
            half4 _GlowColor;
            half  _GlowIntensity;

            // --- Edge ---
            half  _EdgeHighlight;
            half4 _EdgeColor;
            half  _EdgeWidth;
            half  _EdgeIntensity;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            // Remap value from [0,1] to looping [0,1] with pause
            float CalculateScanPosition(float time)
            {
                if (_ScanRepeat > 0)
                {
                    float cycle = _ScanSpeed / _ScanRepeat;
                    float t = frac(time * cycle);
                    // Remap: scan occupies a portion of the cycle, rest is pause
                    float scanPortion = 0.6; // 60% scan, 40% pause
                    if (t > scanPortion)
                        return -1.0; // During pause, scan line is off-screen
                    return t / scanPortion;
                }
                else
                {
                    return frac(time * _ScanSpeed * 0.2);
                }
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 baseColor = texColor * _Color * input.color;

                // ====== Calculate Scan Position ======
                float scanPos;
                #if defined(_USE_TIME)
                    scanPos = CalculateScanPosition(_Time.y);
                #else
                    scanPos = _ScanProgress;
                #endif

                if (_ScanDirection > 0.5)
                scanPos = 1.0 - scanPos;

                // ====== Angle Projection ======
                float angleRad = radians(_ScanAngle);
                float2 scanDir = float2(cos(angleRad), sin(angleRad));
                float scanCoord = dot(input.uv - float2(0.5, 0.5), scanDir) + 0.5;

                // ====== Scan Line ======
                float distToScanLine = abs(scanCoord - scanPos);
                float scanLine = 1.0 - smoothstep(0.0, _ScanWidth, distToScanLine);
                float scanCore = 1.0 - smoothstep(0.0, _ScanWidth * 0.3, distToScanLine);
                float scanMask = scanLine + scanCore * 0.5;

                // ====== Glow Trail ======
                float trailCoord;
                if (_ScanDirection > 0.5)
                    trailCoord = scanCoord - scanPos;
                else
                    trailCoord = scanPos - scanCoord;

                float glowTrail = 0;
                if (trailCoord > 0 && trailCoord < _GlowWidth)
                {
                    glowTrail = 1.0 - (trailCoord / _GlowWidth);
                    glowTrail = pow(glowTrail, 2.0);
                }

                // ====== Edge Highlight ======
                float edgeMask = 0;
                if (_EdgeHighlight > 0.5)
                {
                    float left  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(-_EdgeWidth, 0)).a;
                    float right = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2( _EdgeWidth, 0)).a;
                    float up    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(0,  _EdgeWidth)).a;
                    float down  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(0, -_EdgeWidth)).a;
                    edgeMask = saturate(abs(left - right) + abs(up - down));
                }

                // ====== Alpha-Masked Compositing ======
                half alphaMask = baseColor.a;

                half3 scanContrib = _ScanColor.rgb * scanMask * _ScanIntensity * alphaMask;
                half3 glowContrib = _GlowColor.rgb * glowTrail * _GlowIntensity * alphaMask;
                half3 edgeContrib = _EdgeColor.rgb * edgeMask * _EdgeIntensity * scanLine * alphaMask;

                half3 finalRGB = baseColor.rgb + scanContrib + glowContrib + edgeContrib;
                half  finalA   = alphaMask; // 输出 Alpha 完全由原图决定

                return half4(finalRGB, finalA);
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}