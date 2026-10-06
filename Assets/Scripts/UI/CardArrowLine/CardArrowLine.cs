using UnityEngine;
using Card = CardSystem.Card;

//通用箭头指示线：可连接「卡牌 UI / 3D 物体 / 世界坐标」三种端点，起点与终点各自独立配置
//典型用法：卡牌(CardUI) → 聚集点 ConcentratePoint(3D)；也可 3D 物体 → 3D 物体
//顶点按世界坐标写入(LineRenderer.useWorldSpace = true)，配合 CardArrowLineAlwaysOnTop 材质(ZTest Always)不被棋盘等 3D 物体遮挡
public class CardArrowLine : MonoBehaviour
{
    /// <summary>端点来源模式</summary>
    public enum ArrowPointMode
    {
        CardUI,         //UGUI 卡牌，取 RectTransform 并按其根画布换算屏幕坐标
        Transform3D,    //场景 3D 物体，取 Transform.position
        WorldPosition   //固定世界坐标 Vector3
    }

    [Header("起点")]
    [SerializeField] private ArrowPointMode startMode = ArrowPointMode.CardUI;
    [SerializeField] private Card card;                  //CardUI 模式：被指示的卡牌，SetCard() 会同步 startRT
    [SerializeField] private RectTransform startRT;      //CardUI 模式：卡牌的 RectTransform(由 SetCard 自动同步，也可手动指定)
    [SerializeField] private Transform startTarget;      //Transform3D 模式：起点 3D 物体
    [SerializeField] private Vector3 startWorldPosition; //WorldPosition 模式：起点世界坐标

    [Header("终点")]
    [SerializeField] private ArrowPointMode endMode = ArrowPointMode.Transform3D;
    [SerializeField] private bool autoUseConcentratePoint = true; //Transform3D 模式下未指定 endTarget 时自动指向 ConcentratePoint.instance
    [SerializeField] private Transform endTarget;               //Transform3D 模式：终点 3D 物体
    [SerializeField] private Vector3 endWorldPosition;          //WorldPosition 模式：终点世界坐标

    [Header("曲线参数")]
    [SerializeField] private float curveHeight = 300f; //曲线拱起的最高高度（屏幕像素）
    [SerializeField] private int segments = 30;        //曲线的细分段数，越大越平滑

    [Header("箭头尖端 (可选)")]
    [SerializeField] private Transform arrowHeadPrefab; //箭头尖端预制体，其 +X 视为尖端朝向
    private Transform currentArrowHead;

    [Header("材质 (可选)")]
    [SerializeField] private Material alwaysOnTopMaterial; //ZTest Always 的置顶材质；为空时使用 LineRenderer 上已配置的材质
    private bool hasMaterialWarning;

    [SerializeField] private bool isOpen = false;
    public bool IsOpen {get => isOpen; set => isOpen = value;}

    private LineRenderer lineRenderer;
    private Camera mainCamera;
    private Vector3[] bezierPoints; //缓存数组，避免每帧 GC

    void OnEnable()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true; //本脚本写入的是世界坐标
        ApplyAlwaysOnTopMaterial();
        if (arrowHeadPrefab != null && currentArrowHead == null)
            currentArrowHead = Instantiate(arrowHeadPrefab, transform);
        Refresh();
    }

    void Update()
    {
        Refresh();
    }

    /// <summary>
    /// 立即按当前起点与终点重绘一次箭头。任一端点无效或在相机背后时会隐藏线与箭头
    /// </summary>
    public void Refresh()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();

        Camera cam = GetMainCamera();
        if (cam == null || !TryResolveStartScreenPoint(cam, out Vector2 startScreen) || !TryResolveEndScreenPoint(cam, out Vector2 endScreen, out float endDepth))
        {
            SetLineVisible(false);
            return;
        }

        //两个屏幕点都投影到终点所在深度平面，屏幕上的视觉位置保持不变
        Vector3 p0 = cam.ScreenToWorldPoint(new Vector3(startScreen.x, startScreen.y, endDepth));
        Vector3 p1 = cam.ScreenToWorldPoint(new Vector3(endScreen.x, endScreen.y, endDepth));

        //二次贝塞尔控制点：取中点后沿相机 up 拱起 curveHeight 像素对应的世界长度
        Vector3 controlPoint = (p0 + p1) * 0.5f + cam.transform.up * PixelsToWorldLength(curveHeight, endDepth, cam);

        int count = Mathf.Max(2, segments) + 1;
        if (bezierPoints == null || bezierPoints.Length != count) bezierPoints = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            float t = i / (float)(count - 1);
            bezierPoints[i] = GetQuadraticBezierPoint(t, p0, controlPoint, p1);
        }

        lineRenderer.positionCount = bezierPoints.Length;
        lineRenderer.SetPositions(bezierPoints);
        SetLineVisible(isOpen);

        UpdateArrowHead(cam, p1, bezierPoints[count - 2], endScreen);
    }

    #region 对外接口：起点

    /// <summary>
    /// 设置要指示的卡牌作为起点(自动切到 CardUI 模式)，同步其 RectTransform 并立刻重绘。传 null 可清除
    /// </summary>
    public void SetCard(Card target)
    {
        startMode = ArrowPointMode.CardUI;
        card = target;
        startRT = target != null ? target.GetComponent<RectTransform>() : null;
        Refresh();
    }

    /// <summary>获取当前指示的卡牌，未设置或当前非 CardUI 模式时为 null</summary>
    public Card GetCard() => card;

    /// <summary>设置起点为某个 3D 物体(自动切到 Transform3D 模式)，传 null 可清除</summary>
    public void SetStartTarget(Transform target)
    {
        startMode = ArrowPointMode.Transform3D;
        startTarget = target;
        Refresh();
    }

    /// <summary>设置起点为固定世界坐标(自动切到 WorldPosition 模式)</summary>
    public void SetStartPosition(Vector3 worldPosition)
    {
        startMode = ArrowPointMode.WorldPosition;
        startWorldPosition = worldPosition;
        Refresh();
    }

    /// <summary>直接设置起点模式，配合对应的 Set 接口使用</summary>
    public void SetStartMode(ArrowPointMode mode)
    {
        startMode = mode;
        Refresh();
    }

    /// <summary>获取当前生效的起点模式</summary>
    public ArrowPointMode GetStartMode() => startMode;

    /// <summary>读取起点的世界坐标，无有效起点时返回 false</summary>
    public bool TryGetStartWorldPosition(out Vector3 worldPosition)
    {
        worldPosition = ResolveStartWorldPosition();
        return worldPosition != InvalidWorldPosition;
    }

    /// <summary>读取起点的世界坐标，无有效起点时返回 Vector3.zero</summary>
    public Vector3 GetStartWorldPosition() => TryGetStartWorldPosition(out Vector3 pos) ? pos : Vector3.zero;

    /// <summary>读取起点的屏幕坐标(像素)，可直接用于其它 UI 定位；无有效起点时返回 false</summary>
    public bool TryGetStartScreenPosition(out Vector2 screenPosition)
    {
        Camera cam = GetMainCamera();
        if (cam == null)
        {
            screenPosition = Vector2.zero;
            return false;
        }
        if (!TryResolveStartScreenPoint(cam, out screenPosition)) return false;
        return true;
    }

    /// <summary>读取起点的屏幕坐标(像素)，无有效起点时返回 Vector2.zero</summary>
    public Vector2 GetStartScreenPosition() => TryGetStartScreenPosition(out Vector2 pos) ? pos : Vector2.zero;

    //兼容旧接口名：卡牌相关读取一律走起点
    /// <summary>读取起点世界坐标(等价于 GetStartWorldPosition)</summary>
    public Vector3 GetCardWorldPosition() => GetStartWorldPosition();

    /// <summary>读取起点屏幕坐标(等价于 GetStartScreenPosition)</summary>
    public Vector2 GetCardScreenPosition() => GetStartScreenPosition();

    #endregion

    #region 对外接口：终点

    /// <summary>
    /// 设置终点为某个 3D 物体(自动切到 Transform3D 模式)。
    /// 传 null 且 autoUseConcentratePoint 开启时回退到 ConcentratePoint.instance
    /// </summary>
    public void SetEndTarget(Transform target)
    {
        endMode = ArrowPointMode.Transform3D;
        endTarget = target;
        Refresh();
    }

    /// <summary>设置终点为固定世界坐标(自动切到 WorldPosition 模式)</summary>
    public void SetEndPosition(Vector3 worldPosition)
    {
        endMode = ArrowPointMode.WorldPosition;
        endWorldPosition = worldPosition;
        Refresh();
    }

    /// <summary>直接设置终点模式，配合对应的 Set 接口使用</summary>
    public void SetEndMode(ArrowPointMode mode)
    {
        endMode = mode;
        Refresh();
    }

    /// <summary>获取当前生效的终点模式</summary>
    public ArrowPointMode GetEndMode() => endMode;

    /// <summary>获取当前生效的终点物体(手动终点或聚集点)，无有效目标时为 null</summary>
    public Transform GetEndTarget() => ResolveEndTransform();

    /// <summary>读取终点的世界坐标，无有效终点时返回 false</summary>
    public bool TryGetEndWorldPosition(out Vector3 worldPosition)
    {
        worldPosition = ResolveEndWorldPosition();
        return worldPosition != InvalidWorldPosition;
    }

    /// <summary>读取终点的屏幕坐标(像素)，无有效终点时返回 false</summary>
    public bool TryGetEndScreenPosition(out Vector2 screenPosition)
    {
        Camera cam = GetMainCamera();
        screenPosition = Vector2.zero;
        if (cam == null) return false;
        return TryResolveEndScreenPoint(cam, out screenPosition, out _);
    }

    #endregion

    #region 内部实现

    static readonly Vector3 InvalidWorldPosition = new Vector3(float.NaN, float.NaN, float.NaN);

    //起点世界坐标：按模式取卡牌 RectTransform / 3D 物体 / 固定坐标，无效时返回 NaN 哨兵值
    Vector3 ResolveStartWorldPosition()
    {
        switch (startMode)
        {
            case ArrowPointMode.CardUI:
                return startRT != null ? startRT.position : InvalidWorldPosition;
            case ArrowPointMode.Transform3D:
                return startTarget != null ? startTarget.position : InvalidWorldPosition;
            default:
                return startWorldPosition;
        }
    }

    //起点屏幕坐标：CardUI 必须用卡牌自己所属根画布的相机换算(Overlay 传 null)，
    //否则箭头线挂在 World Space 子画布下时会用错相机，导致线整体飘走
    bool TryResolveStartScreenPoint(Camera cam, out Vector2 screenPoint)
    {
        screenPoint = Vector2.zero;
        if (startMode == ArrowPointMode.CardUI)
        {
            if (startRT == null) return false;
            screenPoint = RectTransformUtility.WorldToScreenPoint(GetCardCanvasCamera(startRT), startRT.position);
            return true;
        }

        Vector3 world = ResolveStartWorldPosition();
        if (world == InvalidWorldPosition) return false;
        Vector3 screen = cam.WorldToScreenPoint(world);
        if (screen.z <= 0f) return false; //在相机背后
        screenPoint = screen;
        return true;
    }

    //终点世界坐标与屏幕坐标，endDepth 为终点在相机前的深度，用于统一整条线的投影平面
    bool TryResolveEndScreenPoint(Camera cam, out Vector2 screenPoint, out float endDepth)
    {
        screenPoint = Vector2.zero;
        endDepth = 0f;

        Vector3 world = ResolveEndWorldPosition();
        if (world == InvalidWorldPosition) return false;

        Vector3 screen = cam.WorldToScreenPoint(world);
        if (screen.z <= 0f) return false; //终点在相机背后时无法正确投影

        screenPoint = screen;
        endDepth = screen.z;
        return true;
    }

    Vector3 ResolveEndWorldPosition()
    {
        if (endMode == ArrowPointMode.WorldPosition) return endWorldPosition;

        Transform end = ResolveEndTransform();
        return end != null ? end.position : InvalidWorldPosition;
    }

    //终点物体：手动终点优先，其次场景中的聚集点单例
    Transform ResolveEndTransform()
    {
        if (endTarget != null) return endTarget;
        if (autoUseConcentratePoint && ConcentratePoint.instance != null) return ConcentratePoint.instance.transform;
        return null;
    }

    //把置顶材质应用到 LineRenderer，避免被棋盘等 3D 物体遮挡
    void ApplyAlwaysOnTopMaterial()
    {
        if (lineRenderer == null) return;

        if (alwaysOnTopMaterial != null)
        {
            lineRenderer.sharedMaterial = alwaysOnTopMaterial;
            return;
        }

        //未配置材质且 LineRenderer 也没有材质(回落默认材质带 ZTest LEqual)时提示一次
        if (lineRenderer.sharedMaterial == null && !hasMaterialWarning)
        {
            hasMaterialWarning = true;
            Debug.LogWarning($"[CardArrowLine] {name} 的 LineRenderer 没有材质，会回落到默认材质(ZTest LEqual)而被 3D 物体遮挡；" +
                             "请把 CardArrowLineAlwaysOnTop 材质拖到 LineRenderer 或本脚本的 alwaysOnTopMaterial 上。", this);
        }
    }

    Camera GetMainCamera()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        return mainCamera;
    }

    //卡牌所属根画布的相机：Screen Space - Overlay 必须传 null，其余传画布相机
    static Camera GetCardCanvasCamera(RectTransform cardRT)
    {
        Canvas canvas = cardRT.GetComponentInParent<Canvas>();
        //沿父级找到根画布(嵌套子画布如 World Space 画布不应作为依据)
        while (canvas != null)
        {
            Canvas root = canvas.rootCanvas;
            if (root == null || root == canvas) break;
            canvas = root;
        }
        if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
        return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
    }

    //把屏幕像素长度换算成指定深度处(垂直于视线的平面)的世界长度
    static float PixelsToWorldLength(float pixels, float depth, Camera cam)
    {
        float worldHeight = cam.orthographic
            ? cam.orthographicSize * 2f
            : 2f * depth * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        return pixels * (worldHeight / Screen.height);
    }

    //箭头尖端贴合曲线终点，正面朝向相机，并在屏幕平面内旋转到末端切线方向
    void UpdateArrowHead(Camera cam, Vector3 tip, Vector3 beforeTip, Vector2 tipScreen)
    {
        if (currentArrowHead == null) return;

        //Overlay 画布下的 UI 尖端，世界坐标即像素坐标，需按屏幕坐标摆放
        RectTransform headRT = currentArrowHead as RectTransform;
        Canvas headCanvas = headRT != null ? headRT.GetComponentInParent<Canvas>() : null;
        if (headCanvas != null && headCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            currentArrowHead.position = new Vector3(tipScreen.x, tipScreen.y, 0f);
        else
            currentArrowHead.position = tip;

        Vector3 dir = tip - beforeTip;
        if (dir.sqrMagnitude < 0.000001f) dir = cam.transform.right;

        float angle = Mathf.Atan2(Vector3.Dot(dir, cam.transform.up), Vector3.Dot(dir, cam.transform.right)) * Mathf.Rad2Deg;
        currentArrowHead.rotation = Quaternion.LookRotation(-cam.transform.forward, cam.transform.up) * Quaternion.Euler(0f, 0f, angle);
    }

    void SetLineVisible(bool visible)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = visible;
            if (!visible) lineRenderer.positionCount = 0;
        }
        if (currentArrowHead != null && currentArrowHead.gameObject.activeSelf != visible)
            currentArrowHead.gameObject.SetActive(visible);
    }

    /// <summary>
    /// 二次贝塞尔曲线公式: B(t) = (1-t)^2 * P0 + 2*(1-t)*t * P1 + t^2 * P2
    /// </summary>
    static Vector3 GetQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1f - t;
        return u * u * p0 + 2f * u * t * p1 + t * t * p2;
    }

    #endregion
}
