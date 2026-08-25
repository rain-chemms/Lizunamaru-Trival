/*
Shader "Custom/BulletEdgeJitter"
{
    Properties
    {
        _MainColor ("主体颜色 (RGBA)", Color) = (1, 1, 0.3, 0.3)
        _EdgeColor ("边缘波动颜色", Color) = (0.0, 1.0, 1.0, 1.0)
        
        _WaveSpeed ("波动速度", Float) = 2.0
        _WaveFreq ("波动频率 (波峰数量)", Float) = 12.0
        _WaveAmp ("波动幅度 (抖动强度)", Float) = 0.05
        _EdgeSmooth ("边缘平滑度 (越小越锐利)", Range(1.0, 8.0)) = 3.0
    }
    
    SubShader
    {
        // 半透明渲染设置，适合子弹罩、护盾
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        // 关闭背面剔除，使护罩从内到外都能看见
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            // 属性变量
            float4 _MainColor;
            float4 _EdgeColor;
            float _WaveSpeed;
            float _WaveFreq;
            float _WaveAmp;
            float _EdgeSmooth;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                
                // 1. 计算视线方向 (世界空间)
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                
                // 2. 计算边缘遮罩 (Fresnel 菲涅尔效应)
                // 法线与视线垂直的地方（即物体的最边缘）值接近 1，中心区域接近 0
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float fresnel = 1.0 - saturate(dot(viewDir, worldNormal));
                fresnel = pow(fresnel, _EdgeSmooth);
                
                // 3. 计算正弦波位移
                // 沿着法线方向进行推拉，产生波动
                float sinWave = sin(worldPos.y * _WaveFreq + _Time.y * _WaveSpeed);
                
                // 4. 只有边缘才会产生位移（核心：乘以 fresnel 权重）
                float3 displacement = worldNormal * sinWave * _WaveAmp * fresnel;
                
                // 5. 应用位移并转换到裁剪空间
                float3 newPos = worldPos + displacement;
                o.pos = mul(UNITY_MATRIX_VP, float4(newPos, 1.0));
                
                o.worldNormal = worldNormal;
                o.viewDir = viewDir;
                o.worldPos = newPos;
                
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // 重新计算片元级别的 Fresnel 用于颜色混合
                float fresnel = 1.0 - saturate(dot(i.viewDir, i.worldNormal));
                fresnel = pow(fresnel, _EdgeSmooth);
                
                // 混合主体颜色和边缘波动颜色
                float3 finalColor = lerp(_MainColor.rgb, _EdgeColor.rgb, fresnel);
                float finalAlpha = _MainColor.a + fresnel * 0.5; // 边缘处稍微变亮/变不透明
                
                return fixed4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
}
*/
/*
Shader "Custom/BulletEdgeWave"
{
    Properties
    {
        _MainColor ("主体颜色", Color) = (1, 1, 0.2, 0.25)
        _EdgeColor ("边缘波动颜色", Color) = (0.0, 1.0, 1.0, 1.0)
        
        //[Header(波动控制)]
        _WaveSpeed ("波动速度", Float) = 5.0
        _WaveFreq ("波动频率 (波纹数量)", Float) = 15.0
        _WaveAmp ("波动强度 (颜色亮度)", Float) = 1.0
        _EdgeWidth ("边缘宽度 (0~1)", Range(0.1, 0.9)) = 0.5
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            float4 _MainColor;
            float4 _EdgeColor;
            float _WaveSpeed;
            float _WaveFreq;
            float _WaveAmp;
            float _EdgeWidth;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                // 保持顶点绝对不动，防止面碎
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // 1. 计算视角方向（从像素指向摄像机）
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                
                // 2. 计算菲涅尔值（Fresnel）：越靠近边缘（法线与视角越垂直），值越接近1
                float fresnel = 1.0 - saturate(dot(normalize(i.normal), viewDir));
                
                // 3. 边缘遮罩：控制边缘的宽度
                float edgeMask = smoothstep(1.0 - _EdgeWidth, 1.0, fresnel);
                
                // 4. 核心：计算正弦波抖动因子（随时间变化）
                // 利用 菲涅尔值 * 频率 产生一圈一圈的波纹
                float wave = sin(_Time.y * _WaveSpeed + fresnel * _WaveFreq);
                
                // 5. 将正弦波转换为0~1的脉动强度，并应用振幅
                float jitter = wave * 0.5 + 0.5; 
                float edgeGlow = jitter * _WaveAmp;
                
                // 6. 合成最终颜色：基础色 + 边缘波动色
                float3 finalColor = _MainColor.rgb;
                
                // 混合边缘发光
                finalColor = lerp(finalColor, _EdgeColor.rgb, edgeMask * (0.5 + edgeGlow));
                
                // 7. 合成最终透明度（边缘处受正弦波抖动影响忽明忽暗）
                float alpha = _MainColor.a + edgeMask * edgeGlow;
                alpha = saturate(alpha);
                
                return float4(finalColor, alpha);
            }
            ENDCG
        }
    }
}
*/
/*
Shader "Custom/BulletEdgeWave"
{
    Properties
    {
        _MainColor ("主体颜色", Color) = (1, 1, 0.2, 0.1)
        _EdgeColor ("边缘波动颜色", Color) = (1, 1, 0.6, 1.0)
        
        //[Header(几何波动控制)]
        _WaveSpeed ("波动速度", Float) = 3.0
        _WaveFreq ("波动频率 (波纹数)", Float) = 12.0
        _WaveAmp ("波动幅度 (建议0.02~0.1)", Range(0.0, 0.2)) = 0.03
        
        //[Header(边缘发光控制)]
        _EdgeWidth ("边缘宽度 (越小边缘越窄)", Range(0.1, 5.0)) = 1.5
        _EdgePower ("边缘发光强度", Range(0.5, 5.0)) = 2.0
        _EdgePulseSpeed ("边缘呼吸速度", Float) = 2.0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        
        // 关闭背面剔除，双面渲染
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            float4 _MainColor;
            float4 _EdgeColor;
            float _WaveSpeed;
            float _WaveFreq;
            float _WaveAmp;
            float _EdgeWidth;
            float _EdgePower;
            float _EdgePulseSpeed;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                
                // 1. 核心防撕裂位移：使用模型局部中心向外的辐射方向，而非模型法线
                // 假设子弹模型的局部中心原点 (0,0,0) 在底部或中心
                float3 center = float3(0.0, 0.0, 0.0);
                float3 expandDir = normalize(v.vertex.xyz - center);
                
                // 2. 沿Y轴高度分布的正弦波（让波纹从下往上扫）
                float waveOffset = sin(v.vertex.y * _WaveFreq + _Time.y * _WaveSpeed);
                
                // 3. 应用几何波动
                v.vertex.xyz += expandDir * waveOffset * _WaveAmp;
                
                o.pos = UnityObjectToClipPos(v.vertex);
                
                // 计算法线和视线方向（用于边缘光）
                o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 1. 计算 Fresnel 边缘遮罩
                float fresnel = 1.0 - saturate(dot(i.normalDir, i.viewDir));
                float edgeMask = pow(fresnel, _EdgeWidth);
                
                // 2. 边缘呼吸脉动效果（正弦波控制边缘强度）
                float pulse = sin(_Time.y * _EdgePulseSpeed) * 0.5 + 0.5; // 0~1 之间循环
                float edgeGlow = edgeMask * _EdgePower * (0.8 + pulse * 0.4);
                
                // 3. 混合主体颜色和边缘颜色
                float3 finalColor = lerp(_MainColor.rgb, _EdgeColor.rgb, edgeGlow);
                float finalAlpha = lerp(_MainColor.a, _EdgeColor.a, edgeMask);
                
                return fixed4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
}
*/ 
/*
Shader "Custom/BulletEdgeWaveY"
{
    Properties
    {
        _MainColor ("主体颜色", Color) = (1, 1, 0.2, 0.15)
        _EdgeColor ("边缘发光颜色", Color) = (1, 1, 0.5, 1.0)
        
        //[Header(向下扭曲控制 - 仅Y轴动)]
        _WaveSpeed ("扭曲速度", Float) = 4.0
        _WaveFreq ("扭曲频率 (波纹数)", Float) = 8.0
        _WaveAmp ("扭曲幅度 (建议0.05~0.2)", Range(0.0, 0.3)) = 0.1
        
        //[Header(边缘发光辅助)]
        _EdgeWidth ("边缘宽度", Range(0.1, 5.0)) = 1.0
        _EdgePulse ("边缘呼吸速度", Float) = 2.0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            float4 _MainColor;
            float4 _EdgeColor;
            float _WaveSpeed;
            float _WaveFreq;
            float _WaveAmp;
            float _EdgeWidth;
            float _EdgePulse;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                
                // 1. 计算 Y 轴的正弦波偏移
                // 公式：sin(高度 * 频率 + 时间 * 速度) * 幅度
                float wave = sin(v.vertex.y * _WaveFreq + _Time.y * _WaveSpeed) * _WaveAmp;
                
                // 2. 将扭曲限制在 Y 轴：
                // 顶点越高（y 越大），向下扭曲（-wave）的幅度就越大；底部几乎不扭曲，保证底座稳定。
                // X 和 Z 轴保持原样，绝对不移动。
                v.vertex.y -= wave * max(0.0, v.vertex.y);
                
                // 3. 空间变换
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. 计算 Fresnel 边缘发光（Rim Light）
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float fresnel = 1.0 - saturate(dot(viewDir, i.worldNormal));
                
                // 2. 边缘呼吸效果
                float edgePulse = sin(_Time.y * _EdgePulse) * 0.2 + 0.8; // 0.8 ~ 1.2 之间波动
                float rim = pow(fresnel, _EdgeWidth) * edgePulse;
                
                // 3. 混合主体颜色和边缘发光
                float3 finalColor = lerp(_MainColor.rgb, _EdgeColor.rgb, rim);
                float finalAlpha = lerp(_MainColor.a, _EdgeColor.a, rim);
                
                return float4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
}
*/
Shader "Custom/BulletEdgeWave_XYZ"
{
    Properties
    {
        _MainColor ("主体颜色", Color) = (1, 1, 0.2, 0.15)
        _EdgeColor ("边缘发光颜色", Color) = (1, 1, 0.5, 1.0)
        
        //[Header(轴心校准 - 模型Pivot不在中心时调整)]
        _CenterOffset ("中心偏移 (X, Z)", Vector) = (0, 0, 0, 0)
        
        //[Header(波动轴向开关 - 1=开启, 0=关闭)]
        _EnableX ("开启 X 轴波动", Float) = 0
        _EnableY ("开启 Y 轴波动", Float) = 1
        _EnableZ ("开启 Z 轴波动", Float) = 0
        
        //[Header(波动参数)]
        _WaveSpeed ("波动速度", Float) = 4.0
        _WaveFreq ("波动频率 (波纹数)", Float) = 8.0
        _WaveAmp ("波动幅度", Range(0.0, 0.5)) = 0.1
        
        //[Header(边缘发光)]
        _EdgeWidth ("边缘宽度", Range(0.1, 5.0)) = 1.0
        _EdgePulse ("边缘呼吸速度", Float) = 2.0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _MainColor;
            fixed4 _EdgeColor;
            float4 _CenterOffset;
            float _EnableX, _EnableY, _EnableZ;
            float _WaveSpeed, _WaveFreq, _WaveAmp;
            float _EdgeWidth, _EdgePulse;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;

                // 1. 计算以校准后中心为原点的局部坐标
                float3 localPos = v.vertex.xyz;
                localPos.x -= _CenterOffset.x;
                localPos.z -= _CenterOffset.z;

                // 2. 计算环绕角度（用于周向波纹）
                float angle = atan2(localPos.z, localPos.x);

                // 3. 计算波动相位（基于角度 + 时间）
                float phase = angle * _WaveFreq + _Time.y * _WaveSpeed;
                float wave = sin(phase) * _WaveAmp;

                // 4. 根据开关决定各轴是否偏移
                float3 offset = 0;
                offset.x = wave * _EnableX;
                offset.y = wave * _EnableY;
                offset.z = wave * _EnableZ;

                // 5. 应用偏移（在原模型坐标上直接加）
                float3 finalPos = v.vertex.xyz + offset;

                o.pos = UnityObjectToClipPos(float4(finalPos, 1.0));
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, float4(finalPos, 1.0)).xyz;
                o.viewDir = UnityWorldSpaceViewDir(o.worldPos);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 计算 Fresnel 边缘光
                float3 N = normalize(i.worldNormal);
                float3 V = normalize(i.viewDir);
                float fresnel = 1.0 - saturate(dot(N, V));
                float edgeMask = pow(fresnel, _EdgeWidth);

                // 边缘呼吸效果
                float pulse = sin(_Time.y * _EdgePulse) * 0.3 + 0.7;
                edgeMask *= pulse;

                // 混合主体颜色和边缘颜色
                fixed4 col = lerp(_MainColor, _EdgeColor, edgeMask);
                col.a = _MainColor.a + edgeMask * _EdgeColor.a;

                return col;
            }
            ENDCG
        }
    }
}