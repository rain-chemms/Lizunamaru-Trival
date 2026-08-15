Shader "Custom/SpriteMouseMask"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)

        [Header(Mouse Mask)]
        _MousePos ("Mouse World Position", Vector) = (0, 0, 0, 0)
        _Radius ("Transparent Radius", Float) = 1.0
        _Feather ("Edge Feather Width", Range(0, 5000)) = 0.5
        _Invert ("Invert Mask (0=Normal, 1=Invert)", Range(0, 1)) = 0
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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
                float3 worldPos : TEXCOORD1;
                float4 color    : COLOR;
            };

            sampler2D _MainTex;
            float4    _MainTex_ST;

            fixed4    _Color;
            float3    _MousePos;
            float     _Radius;
            float     _Feather;
            float     _Invert;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv       = TRANSFORM_TEX(v.uv, _MainTex);
                o.color    = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 采样原始纹理
                fixed4 col = tex2D(_MainTex, i.uv) * _Color * i.color;

                // 计算当前片元到鼠标位置的距离（仅XY平面）
                float dist = distance(i.worldPos.xy, _MousePos.xy);

                // ---- 核心：透明度渐变计算 ----
                // smoothstep(edge0, edge1, x):
                //   x <= edge0  → 0
                //   x >= edge1  → 1
                //   中间平滑过渡
                //
                // dist < _Radius             → maskAlpha = 0 (完全透明)
                // dist > _Radius + _Feather  → maskAlpha = 1 (完全不透明)
                // 中间                        → 平滑渐变
                float maskAlpha = smoothstep(_Radius, _Radius + max(_Feather, 0.001), dist);

                // 支持反转模式
                maskAlpha = lerp(maskAlpha, 1.0 - maskAlpha, _Invert);

                // 将遮罩alpha乘到原始颜色上
                col.a *= maskAlpha;

                return col;
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}