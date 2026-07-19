using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class MaterialShaderChanger : MonoBehaviour
{
    [SerializeField] private Shader targetShader;
    public void SetShader(Shader shader) => targetShader = shader;
    public Shader GetShader() => targetShader;
    void Start()
    {
        FreshMaterialShader();
    }
    
    public void FreshMaterialShader()
    {
        if (targetShader == null)
        {
            Debug.LogError("[MMD4AutoShaderSetter]: Not Set The Valid Shader!");
            return;
        }

        // 获取渲染器组件（MeshRenderer, SkinnedMeshRenderer 等）
        List<Renderer> renderers = GetComponentsInChildren<Renderer>().ToList();
        if (renderers == null)
        {
            Debug.LogError("[MMD4AutoShaderSetter]: This Object Not Have Renderer (and In Children)!");
            return;
        }

        //方式1：修改实例材质（推荐，不影响其他物体）
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials; // 注意：这会创建材质副本
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].shader = targetShader;
            }
            renderer.materials = materials; //必须重新赋值回去！
        }
        //方式2：修改共享材质（影响所有使用该材质的物体，不产生内存开销）
        // Material[] sharedMats = renderer.sharedMaterials;
        // for (int i = 0; i < sharedMats.Length; i++)
        // {
        //     sharedMats[i].shader = targetShader;
        // }
        // renderer.sharedMaterials = sharedMats;
    
    }
}