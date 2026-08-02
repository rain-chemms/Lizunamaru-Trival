Shader "Custom/SpriteWithShadow"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        
        [Header(Shadow Settings)]
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.6)
        _ShadowOffsetX ("Shadow Offset X", Range(-0.1, 0.1)) = 0.02
        _ShadowOffsetY ("Shadow Offset Y", Range(-0.1, 0.1)) = -0.02
        _ShadowBlur ("Shadow Softness", Range(0, 0.05)) = 0 // 0为硬阴影
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "IgnoreProjector"="True"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            
            fixed4 _ShadowColor;
            float _ShadowOffsetX;
            float _ShadowOffsetY;
            float _ShadowBlur;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. 采样原始精灵
                fixed4 original = tex2D(_MainTex, i.texcoord);
                
                // 2. 计算阴影UV（反向偏移）
                // 注意：这里除以 _MainTex_ST.xy 是为了让偏移量不受Tiling影响
                // 如果你希望偏移跟随Tiling缩放，去掉除法即可
                float2 shadowUV = i.texcoord - float2(_ShadowOffsetX, _ShadowOffsetY);
                
                // 3. 采样阴影位置的Alpha
                fixed shadowAlpha = tex2D(_MainTex, shadowUV).a;
                
                // 4. 可选：简单的模糊（多次采样取平均，性能开销会增加）
                // 如果_ShadowBlur为0，跳过此步骤以获得最佳性能
                if (_ShadowBlur > 0)
                {
                    fixed blur = 0;
                    float step = _ShadowBlur * 0.5;
                    blur += tex2D(_MainTex, shadowUV + float2(step, step)).a;
                    blur += tex2D(_MainTex, shadowUV + float2(-step, -step)).a;
                    blur += tex2D(_MainTex, shadowUV + float2(step, -step)).a;
                    blur += tex2D(_MainTex, shadowUV + float2(-step, step)).a;
                    shadowAlpha = saturate((shadowAlpha + blur * 0.25));
                }
                
                // 5. 混合：原图覆盖在阴影之上
                // 只有当原图当前位置透明时，才显示阴影
                fixed4 finalColor;
                finalColor.rgb = lerp(_ShadowColor.rgb * _ShadowColor.a, original.rgb, original.a);
                finalColor.a = max(original.a, shadowAlpha * _ShadowColor.a * (1.0 - original.a));
                
                // 应用顶点色（SpriteRenderer的颜色）
                finalColor *= i.color;
                
                return finalColor;
            }
            ENDCG
        }
    }
}