Shader "Custom/2D_ImageRimLight"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // 边缘光参数
        _RimColor ("Rim Color", Color) = (1, 0.8, 0.2, 1)
        _RimWidth ("Rim Width (Pixels)", Range(0, 50)) = 5.0
        _RimIntensity ("Rim Intensity", Range(0, 5)) = 2.0
        _RimSoftness ("Rim Softness", Range(0, 1)) = 0.5
        _RimBlend ("Rim Blend (0=叠加, 1=替换)", Range(0, 1)) = 0.8
        
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
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
        
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            fixed4 _Color;
            fixed4 _RimColor;
            float _RimWidth;
            float _RimIntensity;
            float _RimSoftness;
            float _RimBlend;
            
            float4 _MainTex_TexelSize;
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif
                return OUT;
            }
            
            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float _AlphaSplitEnabled;
            
            fixed4 SampleSpriteTexture(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);
                #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                if (_AlphaSplitEnabled)
                    color.a = tex2D(_AlphaTex, uv).r;
                #endif
                return color;
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                
                float2 pixelUVSize = _MainTex_TexelSize.xy;
                float2 offset = pixelUVSize * _RimWidth;
                
                float currentAlpha = c.a;
                float alphaUp    = SampleSpriteTexture(IN.texcoord + float2(0, offset.y)).a;
                float alphaDown  = SampleSpriteTexture(IN.texcoord - float2(0, offset.y)).a;
                float alphaLeft  = SampleSpriteTexture(IN.texcoord - float2(offset.x, 0)).a;
                float alphaRight = SampleSpriteTexture(IN.texcoord + float2(offset.x, 0)).a;
                
                float neighborAlpha = max(max(alphaUp, alphaDown), max(alphaLeft, alphaRight));
                float edgeFactor = currentAlpha * (1.0 - neighborAlpha);
                edgeFactor = smoothstep(0, _RimSoftness, edgeFactor);
                
                // 核心逻辑：用 lerp 混合原图颜色和边缘光颜色，_RimBlend=1 时完全替换为边缘光
                c.rgb = lerp(c.rgb, _RimColor.rgb * _RimIntensity, edgeFactor * _RimBlend);
                
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}