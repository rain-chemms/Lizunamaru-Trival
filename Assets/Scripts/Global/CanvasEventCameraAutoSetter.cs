using UnityEngine;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AutoGetCanvas();
        AutoSetEventCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if(alwaysCheck)
        {
            AutoGetCanvas();
            AutoSetEventCamera();
        }    
    }

    private void AutoSetEventCamera()
    {
        if(canvas == null) return;
        Camera camera = canvas.worldCamera;
        if(camera!=null)
        {
            canvas.worldCamera = camera;
        }
    }

    private void AutoGetCanvas()
    {
        if(canvas == null) canvas = GetComponent<Canvas>();
    } 
}
