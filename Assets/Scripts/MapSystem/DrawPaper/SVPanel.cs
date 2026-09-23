using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace MapSystem.DrawPaperSystem
{
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(RectTransform))]
    public class SVPanel : MonoBehaviour, IPointerClickHandler, IDragHandler
    {
        [SerializeField] private RawImage rawImage;
        [SerializeField] private Texture2D tex2d;
        [SerializeField] private RectTransform rectTrans;
        [SerializeField] private Image cursor;//内部的局部光标图像
        [SerializeField] private int texWidth = 256;
        [SerializeField] private int texHeight = 256;

        // 当前色相（由色相条传入）
        private float currentHue = 0f;

        public delegate void ColorChangeDelegate(Color color);
        public event ColorChangeDelegate OnColorChanged;

        void OnEnable()
        {
            if(rawImage == null) rawImage = GetComponent<RawImage>();
            if(rectTrans == null) rectTrans = GetComponent<RectTransform>();
            if(cursor == null) cursor = transform.Find("Cursor").GetComponent<Image>();

            texWidth = Mathf.Abs((int)rectTrans.rect.x);
            texHeight = Mathf.Abs((int)rectTrans.rect.y);
            tex2d = new Texture2D(texWidth, texHeight, TextureFormat.RGB24, true);
            UpdateSVTexture();
            rawImage.texture = tex2d;
            rawImage.texture.wrapMode = TextureWrapMode.Clamp;

        }

        void OnDisable()
        {
            if(tex2d != null) Destroy(tex2d);
        }

        /// <summary>
        /// 设置当前色相并刷新面板
        /// </summary>
        public void SetHue(float hue)
        {
            currentHue = hue;
            UpdateSVTexture();
        }

        /// <summary>
        /// 生成S-V渐变纹理：x轴=饱和度，y轴=明度
        /// </summary>
        private void UpdateSVTexture()
        {
            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    float saturation = (float)x / texWidth;
                    float value = (float)y / texHeight;
                    Color color = Color.HSVToRGB(currentHue, saturation, value);
                    tex2d.SetPixel(x, y, color);
                }
            }
            tex2d.Apply();
        }

        /// <summary>
        /// 点击或拖拽时获取颜色
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            PickColor(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            PickColor(eventData);
        }

        private void PickColor(PointerEventData eventData)
        {
            // 将屏幕坐标转换为面板局部坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTrans, eventData.position, eventData.pressEventCamera,
                out Vector2 localPoint);

            // 限制在面板范围内
            float x = Mathf.Clamp(localPoint.x + rectTrans.sizeDelta.x / 2, 0, rectTrans.sizeDelta.x);
            float y = Mathf.Clamp(localPoint.y + rectTrans.sizeDelta.y / 2, 0, rectTrans.sizeDelta.y);

            // 移动光标
            cursor.rectTransform.anchoredPosition = new Vector2(
                x - rectTrans.sizeDelta.x / 2,
                y - rectTrans.sizeDelta.y / 2);

            // 获取像素颜色
            int pixelX = (int)(x / rectTrans.sizeDelta.x * texWidth);
            int pixelY = (int)(y / rectTrans.sizeDelta.y * texHeight);
            Color pickedColor = tex2d.GetPixel(pixelX, pixelY);

            // 通过委托通知外部
            OnColorChanged?.Invoke(pickedColor);
        }
    }
}
