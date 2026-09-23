using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MapSystem.DrawPaperSystem
{
    [RequireComponent(typeof(RawImage))]
    public class AlphaHue : MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        [SerializeField] private Texture2D tex2d;
        [SerializeField] private int TextureWidth = 270;
        [SerializeField] private int TextureHeight = 8;

        void OnEnable()
        {
            if (rawImage == null) rawImage = GetComponent<RawImage>();
            HandleTextureColor();
        }

        void OnDisable()
        {
            if (tex2d != null) Destroy(tex2d);
        }

        /// <summary>
        /// 生成黑白(透明度)渐变纹理
        /// </summary>
        private void HandleTextureColor()
        {
            // 使用 RGBA32 格式以支持透明度
            tex2d = new Texture2D(TextureWidth, TextureHeight, TextureFormat.RGBA32, true);

            for (int x = 0; x < TextureWidth; x++)
            {
                for (int y = 0; y < TextureHeight; y++)
                {
                    // x轴映射到Alpha值(0~1)，颜色固定为白色
                    float alpha = (float)x / (float)TextureWidth;
                    Color pixColor = new Color(1, 1, 1, alpha);
                    tex2d.SetPixel(x, y, pixColor);
                }
            }
            tex2d.Apply();
            rawImage.texture = tex2d;
            rawImage.texture.wrapMode = TextureWrapMode.Clamp;
        }

        /// <summary>
        /// 根据Slider值获取对应的灰度颜色
        /// </summary>
        public Color GetColorBySliderValue(float value)
        {
            float clampValue = Mathf.Clamp(value, 0.001f, 0.999f);
            Color getColor = tex2d.GetPixel((int)((TextureWidth - 1) * clampValue), 0);
            return getColor;
        }
    }
}
