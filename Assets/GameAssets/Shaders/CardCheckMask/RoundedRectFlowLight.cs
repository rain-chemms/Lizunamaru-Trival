using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 圆角矩形边缘流光效果控制器 (支持外部透明度控制)
/// 挂载到带有 Image 组件的 UI 对象上
/// </summary>
[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class RoundedRectFlowLight : MonoBehaviour
{
    // ==================== 材质管理 ====================
    [Header("材质设置")]
    [Tooltip("指定自定义材质，留空则自动创建实例")]
    [SerializeField] private Material customMaterial;

    private Material _materialInstance;
    private Image _image;

    // ==================== 圆角参数 ====================
    [Header("圆角设置")]
    [Range(0f, 0.5f)]
    [Tooltip("圆角半径，0 = 直角，0.5 = 圆形")]
    [SerializeField] private float radius = 0.1f;

    // ==================== 边框参数 ====================
    [Header("边框设置")]
    [Range(0f, 0.1f)]
    [Tooltip("边框宽度")]
    [SerializeField] private float borderWidth = 0.01f;

    [Tooltip("边框基础颜色")]
    [SerializeField] private Color borderColor = new Color(1f, 1f, 1f, 0.3f);

    // ==================== 流光参数 ====================
    [Header("流光设置")]
    [Tooltip("流光颜色")]
    [SerializeField] private Color flowColor = new Color(0f, 0.8f, 1f, 1f);

    [Range(0f, 0.5f)]
    [Tooltip("流光宽度（占周长的比例）")]
    [SerializeField] private float flowWidth = 0.08f;

    [Range(-5f, 5f)]
    [Tooltip("流光速度，正数顺时针，负数逆时针")]
    [SerializeField] private float flowSpeed = 1.0f;

    [Range(0f, 5f)]
    [Tooltip("流光亮度倍率")]
    [SerializeField] private float flowIntensity = 2.0f;

    [Range(1, 2)]
    [Tooltip("流光数量：1 = 单流光, 2 = 双流光（对称）")]
    [SerializeField] private int flowCount = 1;

    // ==================== 外发光参数 ====================
    [Header("外发光设置")]
    [Range(0f, 0.1f)]
    [Tooltip("外发光扩散范围")]
    [SerializeField] private float glowSpread = 0.02f;

    [Range(0f, 3f)]
    [Tooltip("外发光强度")]
    [SerializeField] private float glowIntensity = 1.0f;

    // ==================== 圆角外部控制 (新增) ====================
    [Header("圆角外部设置")]
    [Range(0f, 1f)]
    [Tooltip("圆角以外区域的透明度，0 = 完全透明(裁切)，1 = 完全不透明")]
    [SerializeField] private float outsideAlpha = 0f;

    [Tooltip("圆角以外区域的底色")]
    [SerializeField] private Color outsideColor = new Color(0f, 0f, 0f, 1f);

    // ==================== Shader 属性 ID 缓存 ====================
    private static readonly int Prop_Radius        = Shader.PropertyToID("_Radius");
    private static readonly int Prop_BorderWidth   = Shader.PropertyToID("_BorderWidth");
    private static readonly int Prop_BorderColor   = Shader.PropertyToID("_BorderColor");
    private static readonly int Prop_FlowColor     = Shader.PropertyToID("_FlowColor");
    private static readonly int Prop_FlowWidth     = Shader.PropertyToID("_FlowWidth");
    private static readonly int Prop_FlowSpeed     = Shader.PropertyToID("_FlowSpeed");
    private static readonly int Prop_FlowIntensity = Shader.PropertyToID("_FlowIntensity");
    private static readonly int Prop_FlowCount     = Shader.PropertyToID("_FlowCount");
    private static readonly int Prop_GlowSpread    = Shader.PropertyToID("_GlowSpread");
    private static readonly int Prop_GlowIntensity = Shader.PropertyToID("_GlowIntensity");
    private static readonly int Prop_OutsideAlpha  = Shader.PropertyToID("_OutsideAlpha");
    private static readonly int Prop_OutsideColor  = Shader.PropertyToID("_OutsideColor");

    // ==================== 生命周期 ====================
    private void Awake()
    {
        _image = GetComponent<Image>();
        EnsureMaterial();
    }

    private void OnEnable()
    {
        _image = GetComponent<Image>();
        EnsureMaterial();
    }

    private void OnDisable()
    {
        // 清理自动实例化的材质
        if (_materialInstance != null && customMaterial == null)
        {
            if (Application.isPlaying)
                Destroy(_materialInstance);
            else
                DestroyImmediate(_materialInstance);
            _materialInstance = null;
        }

        if (_image != null)
            _image.material = null;
    }

    private void Update()
    {
        ApplyProperties();
    }

    // ==================== 内部方法 ====================
    private void EnsureMaterial()
    {
        if (_image == null)
            _image = GetComponent<Image>();

        if (customMaterial != null)
        {
            _image.material = customMaterial;
            _materialInstance = customMaterial;
        }
        else
        {
            if (_materialInstance == null)
            {
                Shader shader = Shader.Find("UI/RoundedRectFlowLight");
                if (shader == null)
                {
                    Debug.LogError("[RoundedRectFlowLight] 找不到 Shader 'UI/RoundedRectFlowLight'，请确认 Shader 文件存在且名称正确。");
                    return;
                }
                _materialInstance = new Material(shader);
                _materialInstance.hideFlags = HideFlags.DontSave;
            }
            _image.material = _materialInstance;
        }
    }

    private void ApplyProperties()
    {
        if (_materialInstance == null) return;

        _materialInstance.SetFloat(Prop_Radius, radius);
        _materialInstance.SetFloat(Prop_BorderWidth, borderWidth);
        _materialInstance.SetColor(Prop_BorderColor, borderColor);
        
        _materialInstance.SetColor(Prop_FlowColor, flowColor);
        _materialInstance.SetFloat(Prop_FlowWidth, flowWidth);
        _materialInstance.SetFloat(Prop_FlowSpeed, flowSpeed);
        _materialInstance.SetFloat(Prop_FlowIntensity, flowIntensity);
        _materialInstance.SetFloat(Prop_FlowCount, flowCount);
        
        _materialInstance.SetFloat(Prop_GlowSpread, glowSpread);
        _materialInstance.SetFloat(Prop_GlowIntensity, glowIntensity);

        // 应用外部透明度参数
        _materialInstance.SetFloat(Prop_OutsideAlpha, outsideAlpha);
        _materialInstance.SetColor(Prop_OutsideColor, outsideColor);
    }

    // ==================== 公开 API ====================
    /// <summary>运行时动态设置流光颜色</summary>
    public void SetFlowColor(Color color) => flowColor = color;

    /// <summary>运行时动态设置流光速度</summary>
    public void SetFlowSpeed(float speed) => flowSpeed = Mathf.Clamp(speed, -5f, 5f);

    /// <summary>运行时动态设置圆角半径</summary>
    public void SetRadius(float r) => radius = Mathf.Clamp(r, 0f, 0.5f);

    /// <summary>运行时动态设置流光宽度</summary>
    public void SetFlowWidth(float width) => flowWidth = Mathf.Clamp(width, 0f, 0.5f);

    /// <summary>运行时设置圆角外部透明度</summary>
    public void SetOutsideAlpha(float alpha) => outsideAlpha = Mathf.Clamp01(alpha);

    /// <summary>运行时设置圆角外部颜色</summary>
    public void SetOutsideColor(Color color) => outsideColor = color;

    /// <summary>暂停/恢复流光动画</summary>
    public void SetPaused(bool paused)
    {
        if (_materialInstance != null)
            _materialInstance.SetFloat(Prop_FlowSpeed, paused ? 0f : flowSpeed);
    }
}