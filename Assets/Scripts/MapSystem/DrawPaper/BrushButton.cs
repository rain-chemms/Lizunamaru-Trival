using UnityEngine;
using UnityEngine.UI;

namespace MapSystem.DrawPaperSystem
{
    [RequireComponent(typeof(Button))]
    public class BrushButton : MonoBehaviour
    {
        //自身按钮
        [SerializeField] private Button button;
        [SerializeField] private DrawManager drawManager;//绘制管理器
        public DrawManager GetDrawManager() => drawManager;

        [SerializeField] private ColorPicker colorPicker;//颜色获取器
        public ColorPicker GetColorPicker() => colorPicker;

        [SerializeField] private Image colorDisplayer;
        public Image GetColorDisplayer() => colorDisplayer;

        void OnEnable()
        {
            if(drawManager == null) drawManager = GetComponentInParent<DrawManager>();
            if(button == null) button = GetComponent<Button>();
            if(colorDisplayer == null) colorDisplayer = GetComponentInChildren<Image>();
            button.onClick.AddListener(OnClick);
        }

        void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            //将颜色获取器中的值传给画笔
            Brush brush = drawManager?.GetBrush();
            Color color = (Color)colorPicker?.GetSelectedColor();
            brush?.SetColor(color);
            if(colorDisplayer != null) colorDisplayer.color = color;
        }
    }
}
