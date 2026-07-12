using UnityEngine;

//该脚本作用为使当前Canvas面朝主摄像机
[RequireComponent(typeof(Canvas))]
public class CanvasMainCameraTowardsFunctioner : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private float lerpSpeed = 10;
    public float GetLerpSpeed() => lerpSpeed;
    public void SetLerpSpeed(float newLerpSpeed) => lerpSpeed = newLerpSpeed;
    void Start()
    {
        if(canvas == null) canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        FaceToMainCamera();
    }
    private void FaceToMainCamera()
    {
        Camera mainCamera = Camera.main;
        if(mainCamera == null) return;
        canvas.worldCamera = mainCamera;//使当前Canvas与主摄像机同步
        Vector3 cameraDir = mainCamera.transform.forward;//摄像机朝向
        //使当前Canvas与摄像机朝向垂直    
        canvas.transform.rotation = Quaternion.Lerp(
            canvas.transform.rotation,
            Quaternion.LookRotation(
                cameraDir,
                Vector3.forward
            ),
            lerpSpeed * Time.deltaTime
        );
    }

    
}
