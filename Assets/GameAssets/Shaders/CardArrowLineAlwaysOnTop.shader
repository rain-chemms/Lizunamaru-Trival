Shader "Custom/CardArrowLineAlwaysOnTop"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Intensity ("Intensity (RGB)", Range(0, 10)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Overlay"
            "IgnoreProjector"="True"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest Always //关键：忽略深度测试，任何 3D 物体(如棋盘)都无法遮挡本线条

        Pass
        {
            Name "ArrowAlwaysOnTop"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                half4  color      : COLOR; //LineRenderer 的 startColor/endColor 渐变走顶点色
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                half4  color       : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            half4 _Color;
            half _Intensity;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                //useWorldSpace = true 时 LineRenderer 顶点已是世界坐标，object space 等同 world space
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half4 col = tex * IN.color * _Color;
                col.rgb *= _Intensity; //强度只作用于 RGB，避免把 alpha 一起推爆
                return col;
            }
            ENDHLSL
        }
    }

    FallBack Off
}
