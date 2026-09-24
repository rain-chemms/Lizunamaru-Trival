Shader "Custom/ImageOutline"
{
    Properties
    {
        [PerRendererData][MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 2
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _AlphaThreshold ("Alpha Threshold", Range(0, 1)) = 0.1
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _MainTex_TexelSize;   // 纹理纹素尺寸，用于将像素宽度转为UV偏移
            fixed4 _Color;
            float _OutlineWidth;
            fixed4 _OutlineColor;
            float _AlphaThreshold;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 left : TEXCOORD1;
                float2 right : TEXCOORD2;
                float2 up : TEXCOORD3;
                float2 down : TEXCOORD4;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;

                // 关键：将像素级宽度转换为UV空间偏移量
                half2 offset = half2(_OutlineWidth, _OutlineWidth) * _MainTex_TexelSize.xy;

                o.left  = o.uv + half2(-offset.x, 0);
                o.right = o.uv + half2( offset.x, 0);
                o.up    = o.uv + half2(0,  offset.y);
                o.down  = o.uv + half2(0, -offset.y);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 采样当前像素颜色
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                // 采样四周偏移位置的Alpha值
                // 乘法逻辑：只要四周任一像素是透明的（alpha≈0），乘积就接近0，说明当前像素处于边缘
                fixed alpha = tex2D(_MainTex, i.left).a
                            * tex2D(_MainTex, i.right).a
                            * tex2D(_MainTex, i.up).a
                            * tex2D(_MainTex, i.down).a;

                // 判断是否为边缘：乘积小于阈值则为边缘区域
                fixed isEdge = step(alpha, _AlphaThreshold);

                // 原图本身透明的区域不显示描边
                isEdge *= col.a;

                // 边缘区域显示描边颜色，非边缘区域显示原图颜色
                fixed3 finalColor = lerp(col.rgb, _OutlineColor.rgb, isEdge);

                return fixed4(finalColor, col.a);
            }
            ENDCG
        }
    }
}