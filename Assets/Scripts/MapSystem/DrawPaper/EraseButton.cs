using UnityEngine;
using UnityEngine.UI;

namespace MapSystem.DrawPaperSystem
{
    [RequireComponent(typeof(Button))]
    public class EraseButton : MonoBehaviour
    {
        //自身按钮
        [SerializeField] private Button button;
        [SerializeField] private DrawManager drawManager;//绘制管理器
        public DrawManager GetDrawManager() => drawManager;

        void OnEnable()
        {
            if(drawManager == null) drawManager = GetComponentInParent<DrawManager>();
            if(button == null) button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            //将颜色Color.clear传给画笔
            Brush brush = drawManager?.GetBrush();
            brush?.SetColor(Color.clear);
        }
    }
}