using UnityEngine;
using UnityEngine.UI;

namespace MapSystem.DrawPaperSystem
{
    public class BrushSizeSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private DrawManager drawManager;//绘制管理器
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            if(drawManager == null) drawManager = Map.instance.GetComponent<DrawManager>();
            if(slider == null) slider = GetComponent<Slider>();
            slider?.onValueChanged.AddListener(OnValueChange);
        }

        void OnDisable()
        {
            slider?.onValueChanged.RemoveListener(OnValueChange);
        }

        private void OnValueChange(float value)
        {
            //设置画笔的大小
            Brush brush = drawManager?.GetBrush();
            brush?.SetSize((int)value);
        }
    }
}