using UnityEngine;
using UnityEngine.UI;

namespace GlobalSystem
{
    /// <summary>
    /// 在 ScrollRect 的 Unrestricted 模式下，基于速度值渐变限制 Content 的拖拽边界。
    /// 挂载到与 ScrollRect 同一个 GameObject 上即可。
    /// </summary>
    public class ScrollRectBoundaryClampSmooth : MonoBehaviour
    {
        [Header("关联组件")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Scrollbar verticalScrollbar;
        [SerializeField] private Scrollbar horizontalScrollbar;

        [Header("边界设置（基于 Content 的 anchoredPosition）")]
        [Tooltip("X 方向最小值（最右可拖到的位置）")]
        [SerializeField] private float minX = 0f;
        [Tooltip("X 方向最大值（最左可拖到的位置）")]
        [SerializeField] private float maxX = 0f;
        [Tooltip("Y 方向最小值（最下可拖到的位置）")]
        [SerializeField] private float minY = 0f;
        [Tooltip("Y 方向最大值（最上可拖到的位置）")]
        [SerializeField] private float maxY = 0f;

        [Header("平滑移动设置")]
        [Tooltip("边界回弹/修正的移动速度（单位：像素/秒）")]
        [SerializeField] private float clampSpeed = 2000f;
        [Tooltip("位置误差小于此值时视为已到达目标，停止计算")]
        [SerializeField] private float positionThreshold = 0.5f;

        [Header("高级选项")]
        [Tooltip("是否限制水平方向")]
        [SerializeField] private bool clampHorizontal = true;
        [Tooltip("是否限制垂直方向")]
        [SerializeField] private bool clampVertical = true;
        [Tooltip("是否同时限制 Scrollbar 的值")]
        [SerializeField] private bool clampScrollbar = true;

        private RectTransform contentRect;
        private Vector2 targetPosition; // 目标修正位置
        private bool isClamping = false; // 是否正在进行边界修正

        private void Awake()
        {
            if (scrollRect == null)
                scrollRect = GetComponent<ScrollRect>();

            contentRect = scrollRect.content;
            targetPosition = contentRect.anchoredPosition;
        }

        private void LateUpdate()
        {
            CheckAndClampContent();
        }

        /// <summary>
        /// 检测是否越界，如果越界则计算目标位置并开启渐变修正。
        /// </summary>
        private void CheckAndClampContent()
        {
            Vector2 currentPos = contentRect.anchoredPosition;
            Vector2 desiredTarget = currentPos;
            bool isOutOfBounds = false;

            // 计算期望的目标位置
            if (clampHorizontal)
            {
                float clampedX = Mathf.Clamp(currentPos.x, minX, maxX);
                if (!Mathf.Approximately(clampedX, currentPos.x))
                {
                    desiredTarget.x = clampedX;
                    isOutOfBounds = true;
                }
            }

            if (clampVertical)
            {
                float clampedY = Mathf.Clamp(currentPos.y, minY, maxY);
                if (!Mathf.Approximately(clampedY, currentPos.y))
                {
                    desiredTarget.y = clampedY;
                    isOutOfBounds = true;
                }
            }

            // 如果检测到越界，更新目标位置并开启修正状态
            if (isOutOfBounds)
            {
                targetPosition = desiredTarget;
                isClamping = true;
            }

            // 如果处于修正状态，执行平滑移动
            if (isClamping)
            {
                float step = clampSpeed * Time.deltaTime;
                Vector2 newPos = Vector2.MoveTowards(currentPos, targetPosition, step);
                contentRect.anchoredPosition = newPos;

                // 同步 Scrollbar
                if (clampScrollbar)
                    SyncScrollbarValues();

                // 判断是否已经到达目标位置（误差范围内）
                if (Vector2.Distance(newPos, targetPosition) < positionThreshold)
                {
                    contentRect.anchoredPosition = targetPosition; // 强制归位，消除微小误差
                    isClamping = false;
                }
            }
        }

        /// <summary>
        /// 将 Content 的当前 anchoredPosition 映射到 Scrollbar 的 normalizedPosition。
        /// </summary>
        private void SyncScrollbarValues()
        {
            Vector2 currentPos = contentRect.anchoredPosition;

            if (clampHorizontal && horizontalScrollbar != null)
            {
                float rangeX = maxX - minX;
                if (!Mathf.Approximately(rangeX, 0f))
                    horizontalScrollbar.value = Mathf.InverseLerp(minX, maxX, currentPos.x);
            }

            if (clampVertical && verticalScrollbar != null)
            {
                float rangeY = maxY - minY;
                if (!Mathf.Approximately(rangeY, 0f))
                    verticalScrollbar.value = Mathf.InverseLerp(minY, maxY, currentPos.y);
            }
        }

        /// <summary>
        /// 运行时动态设置边界。
        /// </summary>
        public void SetBoundary(float minX, float maxX, float minY, float maxY)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
            CheckAndClampContent();
        }

        /// <summary>
        /// 强制触发一次边界检测与修正。
        /// </summary>
        public void ForceClamp()
        {
            CheckAndClampContent();
        }
    }
}