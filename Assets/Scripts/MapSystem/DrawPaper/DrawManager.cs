using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
//绘画面板
namespace MapSystem.DrawPaperSystem
{
    public class DrawManager : MonoBehaviour
    {
        [Header("关联的绘画纸")]
        [SerializeField] private DrawPaper drawPaper;
        public DrawPaper GetDrawPaper() => drawPaper;

        [Header("画笔设置")]
        [SerializeField] private Brush brush = new Brush();
        public Brush GetBrush() => brush;

        [Header("输入系统")]
        [SerializeField] private InputActionAsset inputAsset;
        private InputActionMap drawInputMap;
        private InputAction draw;
        private InputAction clear;
        private InputAction pointer;

        [Header("实时控制信息")]
        [SerializeField] private bool isDrawing = false;
        [SerializeField] private Vector2? lastPos;
        void Update()
        {
            if (isDrawing)
            {
                Vector2 uv = GetPixelUV();
                Texture2D drawTex = drawPaper?.GetDrawTexture();
                if (lastPos.HasValue && lastPos.Value != uv)
                {
                    // 在两点之间插值，避免快速移动时线条断裂
                    DrawLine(lastPos.Value, uv);
                }
                else DrawPoint(uv);

                lastPos = uv;
                drawTex?.Apply(); // 每帧应用一次，优化性能
            }
        }


        void OnEnable()
        {
            InitInput();
            LinkFuncs();
        }

        private void InitInput()
        {
            drawInputMap = inputAsset?.FindActionMap("DrawManager");
            draw = drawInputMap?.FindAction("Draw");
            clear = drawInputMap?.FindAction("Clear");
            pointer = drawInputMap?.FindAction("Pointer");
        }

        private void LinkFuncs()
        {
            draw.started += OnDrawStart;
            draw.canceled += OnDrawEnd;
            clear.performed += OnClear;
        }

        private void OnDrawStart(InputAction.CallbackContext context)
        {
            isDrawing = true;
            lastPos = null;
        }

        private void OnDrawEnd(InputAction.CallbackContext context)
        {
            isDrawing = false;
        }

        private void OnClear(InputAction.CallbackContext context)
        {
            drawPaper.Clear(Color.clear);
        }

        //线条绘制
        private void DrawLine(Vector2 from, Vector2 to)
        {
            Texture2D tex = drawPaper?.GetDrawTexture();
            int width = (int)tex?.width;
            int height = (int)tex?.height;

            Vector2 pixelStart = new Vector2(from.x * width, from.y * height);
            Vector2 pixelEnd = new Vector2(to.x * width, to.y * height);
            brush?.DrawLine_Pixel(tex, Vector2Int.FloorToInt(pixelStart), Vector2Int.FloorToInt(pixelEnd));
        }

        //单点绘制
        private void DrawPoint(Vector2 pos)
        {
            brush.Draw(drawPaper?.GetDrawTexture(), pos);
        }

        /// <summary>
        /// 获取鼠标在纹理上的UV坐标（0~1）
        /// </summary>
        private Vector2 GetPixelUV()
        {
            Vector2 mousePos = pointer.ReadValue<Vector2>();//通过输入系统获取鼠标位置
            RectTransform rt = drawPaper.GetRT();
            Vector2 localPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt, mousePos, null, out localPos);

            localPos += new Vector2(rt.rect.width / 2, rt.rect.height / 2);

            return new Vector2(
                localPos.x / rt.rect.width,
                localPos.y / rt.rect.height);
        }
    }
}