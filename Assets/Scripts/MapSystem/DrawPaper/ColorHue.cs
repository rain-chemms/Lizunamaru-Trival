using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MapSystem.DrawPaperSystem
{
    [RequireComponent(typeof(RawImage))]
    public class ColorHue : MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        [SerializeField] private Texture2D tex2d;
        [SerializeField] private int TextureWidth = 270;
        [SerializeField] private int TextureHeight = 8;

        void OnEnable()
        {
            if(rawImage == null) rawImage = GetComponent<RawImage>();
            HandleTextureColor();
        }

        void OnDisable()
        {
            if(tex2d != null) Destroy(tex2d);
        }

        /// <summary>
        /// 生成色相渐变纹理
        /// </summary>
        private void HandleTextureColor()
        {
            tex2d = new Texture2D(TextureWidth, TextureHeight, TextureFormat.RGB24, true);
            
            for (int x = 0; x <= TextureWidth; x++)
            {
                for (int y = 0; y < TextureHeight; y++)
                {
                    // x轴映射到Hue(0~1)，S和V固定为1
                    Color pixColor = Color.HSVToRGB((float)x / (float)TextureWidth, 1, 1);
                    tex2d.SetPixel(x, y, pixColor);
                }
            }
            tex2d.Apply();
            rawImage.texture = tex2d;
            rawImage.texture.wrapMode = TextureWrapMode.Clamp;
        }

        /// <summary>
        /// 根据Slider值获取色相颜色
        /// </summary>
        public Color GetColorBySliderValue(float value)
        {
            float clampValue = Mathf.Clamp(value, 0.001f, 0.999f);
            Color getColor = tex2d.GetPixel((int)((TextureWidth - 1) * clampValue), 0);
            return getColor;
        }
    }
}
