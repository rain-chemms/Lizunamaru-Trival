using UnityEngine;

namespace GlobalSystem
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasEventCameraAutoSetter : MonoBehaviour
    {
        [SerializeField] private bool alwaysCheck = false;
        public bool IsAlwaysCheck()
        {
            return alwaysCheck;
        }

        public void SetAlwaysCheck(bool always)
        {
            alwaysCheck = always;
        }

        [SerializeField] private Canvas canvas;
        void Start()
        {
            AutoGetCanvas();
            AutoSetEventCamera();
        }

        void Update()
        {
            if (alwaysCheck)
            {
                AutoGetCanvas();
                AutoSetEventCamera();
            }
        }

        private void AutoSetEventCamera()
        {
            if (canvas == null) return;
            canvas.worldCamera = Camera.main;
        }

        private void AutoGetCanvas()
        {
            if (canvas == null) canvas = GetComponent<Canvas>();
        }
    }
}
