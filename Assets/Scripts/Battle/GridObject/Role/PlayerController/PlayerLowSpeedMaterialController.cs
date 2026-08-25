using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace GridObjectSystem.RoleSystem.PlayerSystem
{
    [RequireComponent(typeof(PlayerMoveController))]
    public class PlayerLowSpeedMaterialController : MonoBehaviour
    {
        [SerializeField] private PlayerMoveController moveController;
        //子渲染器列表
        void OnEnable()
        {
            if (moveController == null) moveController = GetComponent<PlayerMoveController>();
            if (renderers == null || renderers.Count <= 0) GetChildRenderersWithOutComponent<PlayerCheckPoint>(transform, renderers);
        }

        [SerializeField] private List<Renderer> renderers = new List<Renderer>();
        private void GetChildRenderersWithOutComponent<T>(Transform node, List<Renderer> result) where T : MonoBehaviour
        {
            // 关键：如果当前节点含有排除脚本，直接返回，不遍历子物体
            if (node.TryGetComponent<T>(out _)) return;
            // 收集当前节点的 Renderer
            if (node.TryGetComponent<Renderer>(out var renderer)) result.Add(renderer);
            // 递归处理子节点
            for (int i = 0; i < node.childCount; i++)
            {
                GetChildRenderersWithOutComponent<T>(node.GetChild(i), result);
            }
        }

        void Update()
        {
            CheckAndSetTheMaterials();
        }

        //检测并切换材质的参数
        [Header("材质参数: 透明度值")]
        [SerializeField] private float lowSpeedAlpha = 0.05f;
        [SerializeField] private float normalSpeedAlpha = 1.0f;
        [Header("材质参数: 透明度阈值")]
        [SerializeField] private float lowSpeedAlphaClip = 0.1f;
        [SerializeField] private float normalSpeedAlphaClip = 0.1f;
        [Header("材质参数: 主纹理阈值")]
        [SerializeField] private float lowSpeedMainTexCutOff = 0.1f;
        [SerializeField] private float normalSpeedMainTexCutOff = 0.1f;
        [Header("材质参数: _RimLightWidth")]
        [SerializeField] private float lowSpeedRimLightWidth = 6.0f;
        [SerializeField] private float normalSpeedRimLightWidth = 3.0f;

        private void CheckAndSetTheMaterials()
        {
            if (moveController == null) return;
            if (renderers == null) return;
            bool isLowSpeed = moveController.GetIsLowSpeed();
            foreach (Renderer rd in renderers)
            {
                //确保材质获取正确
                if (rd == null) continue;
                List<Material> mats = rd.materials.ToList();
                foreach (Material mat in mats)
                {
                    if (mat == null) continue;
                    // 设置材质参数
                    // 是否只是显示边缘
                    bool onlyRim = false;
                    if (mat.HasProperty("_OnlyOutlineAndRim"))
                    {
                        onlyRim = mat.GetFloat("_OnlyOutlineAndRim") > 0.5f;
                    }
                    else
                    {
                        Debug.LogWarning("[PlayerLowSpeedMaterialController]:Material Don't Have \"_OnlyOutlineAndRim\" property!");
                    }
                    // 检测并切换材质
                    if (isLowSpeed != onlyRim)
                    {
                        mat.SetFloat("_OnlyOutlineAndRim", isLowSpeed ? 1.0f : 0.0f);
                        if(isLowSpeed) mat.EnableKeyword("_ONLY_OUTLINE_RIM_ON");
                        else mat.DisableKeyword("_ONLY_OUTLINE_RIM_ON");
                        if (isLowSpeed)//只显示边缘
                        {
                            mat.SetFloat("_Alpha", lowSpeedAlpha);    
                            //mat.SetFloat("_AlphaClip", lowSpeedAlphaClip);
                            //mat.SetFloat("_MainTexCutOff", lowSpeedMainTexCutOff);
                            mat.SetFloat("_RimLightWidth", lowSpeedRimLightWidth);
                        }
                        else//显示正常材质
                        {
                            mat.SetFloat("_Alpha", normalSpeedAlpha);
                            //mat.SetFloat("_AlphaClip", normalSpeedAlphaClip);
                            //mat.SetFloat("_MainTexCutOff", normalSpeedMainTexCutOff);
                            mat.SetFloat("_RimLightWidth", normalSpeedRimLightWidth);
                        }
                    }
                }
            }
        }
    }
}