Shader "Particles/GrayscaleFireParticle"
{
    // Particle fire shader for stretched-billboard flame quads.
    // The flame texture is 24-bit RGB grayscale WITHOUT alpha; the flame shape
    // lives in the luminance channel, so we sample grayscale and use it as opacity.
    // Blending is additive; per-particle animation uses AgePercent (fade over life)
    // and StableRandom (per-particle variation) from custom vertex streams.
    Properties
    {
        _MainTex ("Flame Texture (RGB grayscale)", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1, 0.5, 0.2, 1)
        _Opacity ("Opacity", Range(0, 4)) = 1.5
        _FadeStrength ("Age Fade Strength", Range(0, 2)) = 1.0
        _RandomStrength ("Random Variation Strength", Range(0, 1)) = 0.5
        _SoftParticles ("Soft Particles", Range(0, 1)) = 0
        _SoftDistance ("Soft Fade Distance", Range(0.01, 5)) = 1.0
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
        }
        LOD 100

        Blend One One
        ZWrite Off
        ZTest LEqual
        Cull Off
        Lighting Off

        Pass
        {
            Name "FirePass"
            Tags { "LightMode" = "UniversalForward" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TintColor;
            half _Opacity;
            half _FadeStrength;
            half _RandomStrength;
            half _SoftParticles;
            half _SoftDistance;

            #if defined(SOFTPARTICLES_ON)
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            #endif

            struct appdata_t
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                // Custom vertex streams (must match ParticleSystem renderer):
                // TEXCOORD1 = AgePercent (x), StableRandom (y..w)
                float4 streams : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                half agePercent : TEXCOORD1;
                half randSeed : TEXCOORD2;
                #if defined(SOFTPARTICLES_ON)
                float4 projPos : TEXCOORD3;
                #endif
                UNITY_FOG_COORDS(4)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.agePercent = saturate(v.streams.x);
                o.randSeed = v.streams.y;
                #if defined(SOFTPARTICLES_ON)
                o.projPos = ComputeScreenPos(o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);
                #endif
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Grayscale luminance sampled as opacity (texture has no alpha).
                fixed3 rgb = tex2D(_MainTex, i.texcoord).rgb;
                fixed lum = dot(rgb, fixed3(0.3333, 0.3333, 0.3333));

                // Age fade: 1 at birth -> 0 at end of life.
                half ageFade = 1.0 - saturate(i.agePercent) * _FadeStrength;

                // Per-particle random variation around 1.
                half randVar = 1.0 + (i.randSeed * 2.0 - 1.0) * _RandomStrength;

                half alpha = lum * _Opacity * ageFade * randVar * i.color.a;

                #if defined(SOFTPARTICLES_ON)
                if (_SoftParticles > 0.001)
                {
                    half sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                    half partZ = i.projPos.z;
                    half fade = saturate(_SoftDistance * (sceneZ - partZ));
                    alpha *= lerp(1.0, fade, _SoftParticles);
                }
                #endif

                fixed4 col = fixed4(_TintColor.rgb * i.color.rgb * alpha, alpha);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0, 0, 0, 0));
                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
