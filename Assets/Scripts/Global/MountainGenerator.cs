using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class MountainGenerator : MonoBehaviour
{
    [Header("=== 基础设置 ===")]
    public int seed = 0;
    public float terrainHeightScale = 300f; // 地形最大高度缩放

    [Header("=== 主峰参数 ===")]
    [Range(0.1f, 1.0f)] public float mainPeakRadius = 0.3f;   // 主峰影响半径(0-1)
    [Range(1.0f, 8.0f)] public float mainPeakSteepness = 2.5f; // 主峰陡峭度(指数)
    public Vector2 mainPeakOffset = Vector2.zero;              // 主峰中心偏移(归一化坐标)

    [Header("=== 卫峰(小山峰)参数 ===")]
    [Range(0, 10)] public int subPeakCount = 5;       // 卫峰数量
    [Range(0.05f, 0.5f)] public float subPeakMaxHeightRatio = 0.35f; // 卫峰最大高度占主峰比例
    [Range(0.05f, 0.3f)] public float subPeakRadius = 0.12f;         // 单个卫峰半径
    [Range(0.2f, 0.9f)] public float subPeakMinDistFromCenter = 0.3f; // 卫峰距中心最小距离

    [Header("=== 噪声细节 ===")]
    [Range(1f, 50f)] public float noiseFrequency = 8f;
    [Range(1, 6)] public int noiseOctaves = 4;
    [Range(0.1f, 1.0f)] public float noiseLacunarity = 2.0f;
    [Range(0.1f, 1.0f)] public float noisePersistence = 0.5f;
    [Range(0f, 0.3f)] public float noiseAmplitude = 0.08f; // 噪声对最终高度的扰动幅度

    private Terrain _terrain;
    private System.Random _rng;

    void Awake()
    {
        _terrain = GetComponent<Terrain>();
    }

    /// <summary>
    /// 执行生成，由Editor按钮调用
    /// </summary>
    public void Generate()
    {
        if (_terrain == null) _terrain = GetComponent<Terrain>();

        // 使用当前seed初始化随机数生成器
        _rng = new System.Random(seed);

        int w = _terrain.terrainData.heightmapResolution;
        int h = _terrain.terrainData.heightmapResolution;
        float[,] heights = new float[w, h];

        // 预计算卫峰位置
        var subPeaks = GenerateSubPeakPositions();

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float nx = (float)x / (w - 1);
                float ny = (float)y / (h - 1);

                // 1. 主峰基底 (径向衰减)
                float dx = nx - (0.5f + mainPeakOffset.x);
                float dy = ny - (0.5f + mainPeakOffset.y);
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float mainFalloff = Mathf.Clamp01(1f - dist / mainPeakRadius);
                float mainHeight = Mathf.Pow(mainFalloff, mainPeakSteepness);

                // 2. 卫峰叠加
                float subHeight = 0f;
                foreach (var sp in subPeaks)
                {
                    float sdx = nx - sp.pos.x;
                    float sdy = ny - sp.pos.y;
                    float sDist = Mathf.Sqrt(sdx * sdx + sdy * sdy);
                    float sFalloff = Mathf.Clamp01(1f - sDist / subPeakRadius);
                    float sPeak = Mathf.Pow(sFalloff, mainPeakSteepness * 1.2f) * sp.height;
                    subHeight = Mathf.Max(subHeight, sPeak);
                }

                // 3. 分形噪声细节
                float noiseVal = GetFractalNoise(nx, ny);

                // 4. 合成最终高度
                float baseShape = Mathf.Clamp01(mainHeight + subHeight);
                // 噪声只在有高度的地方生效，避免平地出现噪点坑洞
                float finalHeight = baseShape + noiseVal * noiseAmplitude * baseShape;

                heights[y, x] = Mathf.Clamp01(finalHeight);
            }
        }

        _terrain.terrainData.SetHeights(0, 0, heights);

        // 同步更新Terrain的物理高度
        var td = _terrain.terrainData;
        td.size = new Vector3(td.size.x, terrainHeightScale, td.size.z);

        Debug.Log($"[MountainGenerator] 山峰生成完成 Seed={seed}");
    }

    /// <summary>
    /// 生成卫峰的随机位置和高度
    /// </summary>
    private (Vector2 pos, float height)[] GenerateSubPeakPositions()
    {
        var peaks = new (Vector2 pos, float height)[subPeakCount];
        for (int i = 0; i < subPeakCount; i++)
        {
            float angle = (float)(_rng.NextDouble() * Mathf.PI * 2);
            float dist = subPeakMinDistFromCenter +
                         (float)_rng.NextDouble() * (0.5f - subPeakMinDistFromCenter);

            float px = 0.5f + mainPeakOffset.x + Mathf.Cos(angle) * dist;
            float py = 0.5f + mainPeakOffset.y + Mathf.Sin(angle) * dist;

            // 卫峰高度随机，但远低于主峰
            float h = (float)(_rng.NextDouble() * subPeakMaxHeightRatio);

            peaks[i] = (new Vector2(px, py), h);
        }
        return peaks;
    }

    /// <summary>
    /// 基于种子的分形布朗运动(FBM)噪声
    /// </summary>
    /*
    private float GetFractalNoise(float x, float y)
    {
        float value = 0f;
        float amplitude = 1f;
        float frequency = noiseFrequency;
        float maxValue = 0f;

        // 用seed偏移噪声采样坐标，使不同seed产生不同噪声
        float offsetX = seed * 137.5f;
        float offsetY = seed * 241.3f;

        for (int i = 0; i < noiseOctaves; i++)
        {
            value += Mathf.PerlinNoise(
                (x + offsetX) * frequency, 
                (y + offsetY) * frequency
            ) * amplitude;
            
            maxValue += amplitude;
            amplitude *= noisePersistence;
            frequency *= noiseLacunarity;
        }

        // 归一化到 -0.5 ~ 0.5
        return (value / maxValue) - 0.5f;
    }
    */
    /// <summary>
    /// 基于世界空间和种子的独立分形噪声
    /// </summary>
    private float GetFractalNoise(float normalizedX, float normalizedY)
    {
        float value = 0f;
        float amplitude = 1f;
        float frequency = noiseFrequency;
        float maxValue = 0f;

        // 👇 核心修改：获取当前Terrain的世界坐标作为空间唯一标识
        Vector3 worldPos = _terrain.transform.position;

        // 👇 使用非线性位运算哈希，彻底打散Seed与世界坐标的关联
        // 避免简单的线性乘法导致的噪声周期重合
        uint hash = (uint)(seed * 2654435761u); // Knuth乘法哈希
        hash ^= (uint)(worldPos.x * 12345.6789f);
        hash ^= (uint)(worldPos.z * 98765.4321f);
        hash = ((hash >> 16) ^ hash) * 0x45d9f3b;
        hash = ((hash >> 16) ^ hash) * 0x45d9f3b;
        hash = (hash >> 16) ^ hash;

        // 将哈希值转换为无理数偏移量，避开Perlin Noise的整数网格陷阱
        float offsetX = (hash & 0xFFFF) / 65535f * 1000f + 0.5f;
        float offsetY = ((hash >> 16) & 0xFFFF) / 65535f * 1000f + 0.5f;

        for (int i = 0; i < noiseOctaves; i++)
        {
            value += Mathf.PerlinNoise(
                (normalizedX + offsetX) * frequency,
                (normalizedY + offsetY) * frequency
            ) * amplitude;

            maxValue += amplitude;
            amplitude *= noisePersistence;
            frequency *= noiseLacunarity;
        }

        return (value / maxValue) - 0.5f;
    }
}