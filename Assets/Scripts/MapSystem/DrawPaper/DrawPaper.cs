using UnityEngine;
using UnityEngine.UI;

namespace MapSystem.DrawPaperSystem
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(RawImage))]
    public class DrawPaper : MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        public RawImage GetRawImage() => rawImage;
        
        [SerializeField] private RectTransform rectTransform;
        public RectTransform GetRT() => rectTransform;

        [SerializeField] private Texture2D drawTexture;
        public Texture2D GetDrawTexture() => drawTexture;

        private void OnEnable()
        {
            if(rawImage == null) rawImage = GetComponent<RawImage>();
            if(rectTransform == null) rectTransform = GetComponent<RectTransform>();
            InitDrawTexture();
        }

        private void InitDrawTexture()
        {
            Destroy(drawTexture);//销毁之前的drawTexture
            //获取长宽
            RectTransform rt = rawImage.rectTransform;
            int width = (int)rt.rect.width;
            int height = (int)rt.rect.height;
            //创建drawTexture并设置到RawImage
            drawTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            rawImage.texture = drawTexture;
            Clear(Color.clear);//清空画布设置为透明
        }

        //用一种颜色清空画布
        public void Clear(Color clearColor)
        {
            if(drawTexture == null) return;
            //定义颜色矩阵
            Color[] colors = new Color[drawTexture.width * drawTexture.height];
            //为每个像素颜色赋值
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = clearColor;
            }
            drawTexture.SetPixels(colors);
            drawTexture.Apply();
        }
    }
}