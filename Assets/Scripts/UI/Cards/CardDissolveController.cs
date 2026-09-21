using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardDissolveController : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private Image targetImage;//溶解目标
    [Header("溶解进度")]
    [SerializeField] private float dissolveProgress = 0.5f;
    public void SetDissolveProgress(float progress) => dissolveProgress = progress; 
    public float GetDissolveProgress() => dissolveProgress;//获取溶解进度
    [SerializeField] private Material dissolveMaterial;//dissolve材质
    [SerializeField] private Material materialInstance;
    [SerializeField] private Material sourceMaterial;

    void Start()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        sourceMaterial = targetImage?.material;
    }

    public void SetDissolveMaterial()
    {
        DestroyMaterialInstance();
        // 关键: 使用 materialInstance 避免修改共享材质影响其他UI
        if(targetImage != null)
        {
            materialInstance = Instantiate(dissolveMaterial);
            targetImage.material = materialInstance;    
        }
    }

    public void RevertMaterial()
    {
        targetImage.material = sourceMaterial;
        DestroyMaterialInstance();
    }

    void LateUpdate()
    {
        if(materialInstance!=null) materialInstance?.SetFloat("_Threshold", dissolveProgress);     
    }   

    void OnDestroy()
    {
        DestroyMaterialInstance();
    }

    void DestroyMaterialInstance()
    {
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
        materialInstance = null;
    }
}