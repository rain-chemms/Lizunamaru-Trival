Shader "2D/Starfield_URP_Unlit"
{
    Properties
    {
        [HDR] _TintColor ("Tint Color", Color) = (1, 1, 1, 1)
        _Density ("Star Density", Range(10, 200)) = 50
        _Size ("Star Size", Range(0.01, 0.5)) = 0.1
        _TwinkleSpeed ("Twinkle Speed", Range(0, 10)) = 2.0
        _Brightness ("Brightness", Range(1, 5)) = 2.0
        _MaskThreshold ("Density Threshold", Range(0.8, 0.99)) = 0.92
        _ScrollSpeed ("Scroll Speed", Vector) = (0.1, 0.05, 0, 0)
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
            Name "StarfieldPass"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // ==================== 材质属性 CBUFFER ====================
            CBUFFER_START(UnityPerMaterial)
                half4 _TintColor;
                float _Density;
                float _Size;
                float _TwinkleSpeed;
                float _Brightness;
                float _MaskThreshold;
                float4 _ScrollSpeed;
            CBUFFER_END

            // ==================== 内联核心算法 ====================

            // 伪随机哈希：将2D网格坐标映射到[0,1]
            inline float StarHash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            // ==================== 顶点/片元结构体 ====================
            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 worldXY    : TEXCOORD0;
            };

            // ==================== 顶点着色器 ====================
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                // ★ 必须用世界坐标XY，避免星星粘在Sprite上
                output.worldXY = mul(UNITY_MATRIX_M, input.positionOS).xy;
                return output;
            }

            // ==================== 片元着色器 ====================
            half4 frag(Varyings input) : SV_Target
            {
                float time = _Time.y;
                float2 worldPos = input.worldXY;

                // 1. 缩放 + 滚动偏移
                float2 uv = worldPos * _Density + _ScrollSpeed.xy * time;

                // 2. 网格分割：整数部分=格子ID，小数部分=格内局部坐标
                float2 cell = floor(uv);
                float2 localUV = frac(uv) - 0.5; // 中心化到 [-0.5, 0.5]

                // 3. 每格独立随机值 & 星星在格内的随机偏移
                float rnd = StarHash(cell);
                float2 starOffset = float2(
                    StarHash(cell + float2(1.0, 0.0)),
                    StarHash(cell + float2(0.0, 1.0))
                ) - 0.5;

                // 4. ★ 圆形距离场（Length是方变圆的关键）
                float dist = length(localUV - starOffset * 0.8);

                // 5. 圆形星星形状（Edge1=size, Edge2=0 → 中心亮边缘暗）
                float shape = smoothstep(_Size, 0.0, dist);

                // 6. 密度遮罩（仅部分格子生成星星）
                float mask = step(_MaskThreshold, rnd);

                // 7. 闪烁动画（每颗星相位不同，最暗不低于0.6）
                float twinkle = sin(time * _TwinkleSpeed + rnd * 6.2831853) * 0.5 + 0.5;
                twinkle = lerp(0.6, 1.0, twinkle);

                // 8. 组合最终亮度
                float alpha = shape * mask * twinkle * _Brightness;

                // 9. 应用颜色 tint
                half3 color = _TintColor.rgb * alpha;
                half finalAlpha = _TintColor.a * saturate(alpha);

                return half4(color, finalAlpha);
            }
            ENDHLSL
        }
    }
}