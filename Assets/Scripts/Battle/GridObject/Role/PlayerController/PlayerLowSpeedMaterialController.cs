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
            //是否开启将粒子系统的物体加入白名单
            if(autoAddParticleSystemToWhiteList)
            {
                foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
                {
                    if(ps == null) continue;
                    Transform t = ps?.gameObject.transform;
                    if(t!= null && !(bool)whiteList?.Contains(t)) whiteList?.Add(t);
                }
            }
            
            //是否自动补全白名单
            if(autoIncludeWhiteListChildren)
            {
                foreach(Transform tf in whiteList.ToList())
                {
                    if(tf == null) continue;
                    foreach(Transform child in tf.GetComponentsInChildren<Transform>())
                    {
                        if(child == null) continue;
                        if(!(bool)whiteList?.Contains(child))whiteList.Add(child);
                    }
                }
            }
            RemoveTheRendererInWhiteList();
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

        // 移除白名单中的 Renderer
        private void RemoveTheRendererInWhiteList()
        {
            foreach (Renderer rd in renderers?.ToList())
            {
                if(rd == null) continue;
                Transform rdTf = rd?.gameObject.transform;
                if(rdTf == null) continue;
                if((bool)whiteList?.Contains(rdTf)) renderers?.Remove(rd);
            }
        }

        void Update()
        {
            CheckAndSetTheMaterials();
        }
        [SerializeField] private List<Transform> whiteList = new List<Transform>();
        public List<Transform> GetWhiteList() => whiteList;
        public List<Transform> GetWhiteList_Copy() => whiteList.ToList();
        [SerializeField] private bool autoIncludeWhiteListChildren = true;
        public bool IsAutoIncludeWhiteListChildren() => autoIncludeWhiteListChildren;
        public void SetAutoIncludeWhiteListChildren(bool yes) => autoIncludeWhiteListChildren = yes;
        [SerializeField] private bool autoAddParticleSystemToWhiteList = true;
        public bool IsAutoAddParticleSystemToWhiteList() => autoAddParticleSystemToWhiteList;
        public void SetAutoAddParticleSystemToWhiteList(bool yes) => autoAddParticleSystemToWhiteList = yes;
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
            //RemoveTheRendererInWhiteList();//移除白名单中的材质
            foreach (Renderer rd in renderers)
            {
                //确保材质获取正确
                if (rd == null) continue;
                //if ((bool)whiteList?.Contains(rd.gameObject.transform)) continue;//二次检测白名单物体
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