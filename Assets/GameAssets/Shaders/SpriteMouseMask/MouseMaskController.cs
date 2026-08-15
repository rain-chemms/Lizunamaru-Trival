using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;
/// <summary>
/// 将鼠标世界坐标实时传递给 SpriteMouseMask Shader
/// 挂载方式（二选一）：
///   1. 挂到 Sprite 对象上 → 自动获取自身 Material
///   2. 挂到任意对象上 → 手动拖入 targetMaterial
/// </summary>
//[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class MouseMaskController : MonoBehaviour
{
    [SerializeField] private Image image;
    [Header("引用")]
    [Tooltip("如果不填，则自动使用本物体 SpriteRenderer 的材质")]
    [SerializeField] private Material _mat;
    public Material GetMaterial() => _mat;
    [Header("参数（可在运行时调节）")]
    [Min(0f)]
    [SerializeField] private float radius = 1.0f;
    public float SetRadius(float r) => radius = r;
    public float GetRadius() => radius;
    [SerializeField] private float radiusLerpSpeed = 1.0f;
    public float GetRadiusLerpSpeed() => radiusLerpSpeed;
    [Min(0f)]
    [SerializeField] private float feather = 0.5f;
    public float GetFeather() => feather;
    public void SetFeather(float feather) => this.feather = feather;
    [Tooltip("反转遮罩：勾选后圆形内不透明，圆形外透明")]
    [SerializeField] private bool invert = false;
    public bool GetInvert() => invert;
    public void SetInvert(bool invert) => this.invert = invert;
    [Header("相机")]
    [Tooltip("用于屏幕→世界坐标转换的相机，留空则用 Camera.main")]
    [SerializeField] private Camera renderCamera;
    public Camera GetRenderCamera() => renderCamera;
    public void SetRenderCamera(Camera cam = null) => renderCamera = cam == null ? Camera.main : cam;
    [Header("鼠标位置缓动的速度")]
    [SerializeField] private float mouseLerpSpeed = 10.0f;
    [Header("相关联的输入系统")]
    [SerializeField] private InputActionAsset inputSystem = null;
    [NonSerialized] private InputActionMap nightBlindness = null;//鼠标位置的映射
    [NonSerialized] private InputAction centerPosition = null;
    // 输入系统初始化
    void OnEnable()
    {
        if(image == null) image = GetComponent<Image>();
        if(renderCamera == null) renderCamera = Camera.main;
        _mat = image?.material;
        nightBlindness = inputSystem?.FindActionMap("NightBlindness");
        centerPosition = nightBlindness?.FindAction("CenterPosition");
        nightBlindness?.Enable();
    }

    private static readonly int MousePosID = Shader.PropertyToID("_MousePos");
    private static readonly int RadiusID = Shader.PropertyToID("_Radius");
    private static readonly int FeatherID = Shader.PropertyToID("_Feather");
    private static readonly int InvertID = Shader.PropertyToID("_Invert");
    void Update()
    {
        if (_mat == null) return;
        // 获取相机
        if (renderCamera == null) return;
        Vector2 ctrPos = (Vector2)centerPosition?.ReadValue<Vector2>();
        Debug.Log("ctrPos: x:"+ ctrPos.x +" y:" + ctrPos.y);

        
        RectTransform rt = image.rectTransform;
        Canvas rootCanvas = rt.GetComponentInParent<Canvas>().rootCanvas;
        // ★ 核心：lossyScale 包含了 Canvas Scaler 缩放 + 父级缩放 + 自身 Scale
        Vector3 lossyScale = rt.lossyScale;

        // rect.size 是布局尺寸，乘以 lossyScale 得到实际渲染像素
        Vector2 widthAndHeight = new Vector2(
            rt.rect.width * Mathf.Abs(lossyScale.x),
            rt.rect.height * Mathf.Abs(lossyScale.y)
        );

        // 对于2D正交相机，Z值用相机到Sprite的距离
        float zDist = Mathf.Abs((float)renderCamera?.transform.position.z - transform.position.z);
        //Vector3 mouseWorld = renderCamera.ScreenToWorldPoint(mouseScreen);
        //Debug.Log("mouseWorld: x:"+ mouseWorld.x +" y:" + mouseWorld.y);
        // 获取旧的值
        float oldRadius = _mat.GetFloat(RadiusID);
        float nowRadius = Mathf.Lerp(oldRadius, radius, radiusLerpSpeed * Time.deltaTime);
        _mat.SetFloat(RadiusID, nowRadius);//设置半径
        // 传入Shader
        Vector4 oldMousePos = _mat.GetVector(MousePosID);
        Vector4 targetMousePos = new Vector4(ctrPos.x - widthAndHeight.x / 2, ctrPos.y - widthAndHeight.y / 2 , zDist, 0.0f);
        Vector4 newMousePos = Vector4.Lerp(oldMousePos,targetMousePos, mouseLerpSpeed * Time.deltaTime);
        _mat.SetVector(MousePosID, newMousePos);//设置遮罩位置
        //其他属性直接设置
        _mat.SetFloat(FeatherID, feather);
        _mat.SetFloat(InvertID, invert ? 1f : 0f);
    }
}
