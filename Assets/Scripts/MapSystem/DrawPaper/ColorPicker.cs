using UnityEngine;
using UnityEngine.UI;

namespace MapSystem.DrawPaperSystem
{
    public class ColorPicker : MonoBehaviour
    {
        public Slider hueSlider;
        public SVPanel svPanel;
        public Image previewImage;
        public Slider alphaSlider; //可选

        private Color selectedColor = Color.white;

        void Start()
        {
            // 色相条变化 → 更新SV面板色相
            hueSlider.onValueChanged.AddListener(OnHueChanged);

            // SV面板选色 → 更新预览
            svPanel.OnColorChanged += OnSVColorChanged;

            alphaSlider.onValueChanged.AddListener(OnAlphaChanged);
            // 初始化
            OnHueChanged(hueSlider.value);
        }

        private void OnHueChanged(float value)
        {
            svPanel.SetHue(value);
        }

        private void OnSVColorChanged(Color color)
        {
            selectedColor = color;
            selectedColor.a = alphaSlider != null ? alphaSlider.value : 1f;
            previewImage.color = selectedColor;
        }

        private void OnAlphaChanged(float value)
        {
            selectedColor.a = value;
            previewImage.color = selectedColor;
        }

        /// <summary>
        /// 对外暴露获取选中颜色的方法
        /// </summary>
        public Color GetSelectedColor()
        {
            return selectedColor;
        }
    }
}
