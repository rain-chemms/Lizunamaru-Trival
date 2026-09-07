Shader "Custom/URP/3D_Outline_Rim"
{
    Properties
    {
        [HDR][MainColor] _BaseColor("Base Color (HDR)", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}

        [Header(Rim Glow)]
        [HDR] _RimColor("Rim Color (HDR)", Color) = (1, 3, 6, 1)
        _RimPower("Rim Power", Range(0.1, 10)) = 3
        _RimIntensity("Rim Intensity", Range(0, 20)) = 4
        // ★ 新增：Rim 独立 Alpha
        _RimAlpha("Rim Alpha", Range(0, 1)) = 1

        [Header(Transparency)]
        [Toggle(_ALPHATEST_ON)] _AlphaClip("Alpha Clipping", Float) = 0
        _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "DisableBatching"="True"
        }

        Pass
        {
            Name "ForwardLitTransparentSepAlpha"
            Tags { "LightMode"="UniversalForward" }

            // ★ 改用预乘 Alpha + 加法混合
            // 原因：SrcAlpha/OneMinusSrcAlpha 下，当 baseAlpha=0 时整个像素被丢弃，
            //       Rim 也无法显示。预乘+加法允许 base 全透时 Rim 仍然叠加。
            Blend One OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _ALPHATEST_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float3 viewDirWS   : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _RimColor;
                float  _RimPower;
                float  _RimIntensity;
                float  _RimAlpha;       // ★
                float  _Cutoff;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(positionWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

                // ★ Base 通道：预乘 Alpha
                // 预乘后 baseRGB 自带透明度权重，与 Blend One OneMinusSrcAlpha 配合
                half baseAlpha = baseMap.a * _BaseColor.a;
                half3 baseRGB = baseMap.rgb * _BaseColor.rgb * baseAlpha;

                #ifdef _ALPHATEST_ON
                    clip(baseAlpha - _Cutoff);
                #endif

                half3 N = normalize(IN.normalWS);
                half3 V = normalize(IN.viewDirWS);

                // 光照（已预乘 baseAlpha）
                Light mainLight = GetMainLight();
                half NdotL = saturate(dot(N, mainLight.direction));
                half3 lit = baseRGB * (NdotL * mainLight.color + SampleSH(N));

                // ★ Rim 通道：完全独立的 Alpha 控制
                // RimAlpha 不影响 baseAlpha，baseAlpha=0 时 Rim 依然可见
                half fresnel = pow(1.0 - saturate(dot(N, V)), _RimPower);
                half3 rim = _RimColor.rgb * fresnel * _RimIntensity * _RimAlpha;

                // ★ 最终合成
                // RGB: lit(已预乘) + rim(独立) → 两者均可 >1 → Bloom 生效
                // A:   仅由 baseAlpha 决定深度排序/遮挡关系
                //      Rim 不参与 Alpha，避免 Rim 区域意外遮挡后方物体
                half3 finalColor = lit + rim;
                half finalAlpha = baseAlpha;

                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
}