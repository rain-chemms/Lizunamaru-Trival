Shader "Sprites/URP_ProfileArtStencilWrite"
{
    // ============================================================
    //  用途
    //  角色立绘（SpriteRenderer）在同一次绘制中完成两件事：
    //    1. 向模板缓冲写入 _StencilRef（标记角色轮廓）
    //    2. 正常输出立绘颜色
    //  再让面板背景材质 UI/URP_BackgroundBlur 设置
    //      _Stencil         = 与本材质 _StencilRef 相同的值
    //      _StencilComp     = NotEqual (6)
    //      _StencilReadMask = 与本材质 _StencilWriteMask 相同的值
    //  背景便会在角色轮廓处被"挖空"，不再遮挡立绘。
    //
    //  ★ 为什么是单 Pass ★
    //  早期版本拆成"Pass1 写模板 / Pass2 出颜色"，实测 SpriteRenderer
    //  的绘制路径只提交了第一个 Pass，导致颜色完全丢失、屏幕上只剩
    //  一个被挖空的轮廓。单 Pass 下模板写入与颜色输出天然同步，
    //  不存在该问题。
    //
    //  ★ 单 Pass 的取舍 ★
    //  模板写入发生在光栅化阶段、无法按 alpha 精细控制，只能靠片元
    //  clip 决定哪些像素写模板，因此穿透轮廓的边缘会比立绘本体硬一点。
    //  用 _AlphaCutoff 调节：
    //    调小 -> 角色边缘更完整柔和，但穿透区略大于角色，
    //            周围会多露出一小圈未模糊的场景
    //    调大 -> 穿透区更贴合角色，但角色的半透明边缘会被背景压住
    //
    //  ★ 生效前提（不满足则穿透静默失效）★
    //  模板值必须能从主相机通道保留到 Canvas 绘制通道。通用管线一旦
    //  走"中间渲染纹理 + 末尾 Blit"，Blit 只拷颜色、模板清零。
    //  本项目 PC_RPAsset 开启了 Opaque Texture（背景模糊依赖它，不能关），
    //  因此必然存在中间纹理；要让模板共享，Canvas 必须与主相机处于
    //  同一渲染目标——即给界面根 Canvas 绑定主相机（Screen Space - Camera）。
    //  项目已有 GlobalSystem.CanvasEventCameraAutoSetter 可直接复用。
    // ============================================================

    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Space(10)]
        _AlphaCutoff ("Stencil Alpha Cutoff", Range(0, 1)) = 0.5

        [Space(10)]
        _StencilRef ("Stencil Ref", Range(0, 255)) = 1
        _StencilWriteMask ("Stencil Write Mask", Range(0, 255)) = 1

        [Space(10)]
        [HideInInspector] _RendererColor ("Renderer Color", Color) = (1,1,1,1)
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

        // 在角色可见像素上写入模板标记。
        // 只占用低位，避免与 UGUI Mask 使用的高位冲突。
        Stencil
        {
            Ref [_StencilRef]
            WriteMask [_StencilWriteMask]
            Comp Always
            Pass Replace
            Fail Keep
            ZFail Keep
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGBA

        // 注意：本通道刻意不带 LightMode 标签。
        // 本项目精灵走的是与内置 Sprites-Default 相同的无标签提交路径，
        // 加上 UniversalForward 等标签反而可能被当前渲染器排除。
        Pass
        {
            Name "ProfileArtStencilWrite"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // _RendererColor / _MainTex_ST 由 SpriteRenderer 通过
            // MaterialPropertyBlock 逐实例写入，必须声明为普通 uniform；
            // 放进 CBUFFER 会导致覆盖失效（表现为颜色异常或变黑）。
            float4 _MainTex_ST;
            half4  _RendererColor;

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half  _AlphaCutoff;
            CBUFFER_END

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                half4  color  : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                half4  color      : COLOR;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.vertex.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color * _Color * _RendererColor;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half4 c = texColor * IN.color;

                // 被 discard 的片元不写颜色、深度与模板，
                // 因此模板只会落在角色实际可见的轮廓内部
                clip(c.a - _AlphaCutoff);

                return c;
            }
            ENDHLSL
        }
    }

    // 兜底通道不带模板写入。若本文件编译失败回退到这里，
    // 立绘能正常显示但穿透效果会静默消失——排查时请优先看本文件的编译错误。
    Fallback "Sprites/Default"
}
