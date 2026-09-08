using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace GlobalSystem
{
    public class MMD4AutoShaderSetter : MonoBehaviour
    {
        [SerializeField] private Shader targetShader;
        public void SetShader(Shader shader) => targetShader = shader;
        public Shader GetShader() => targetShader;
        [SerializeField] private List<Transform> whiteList = new List<Transform>();//白名单,在白名单中的游戏物体及其子物体不自动设置
        public List<Transform> GetWhiteList() => whiteList;
        public List<Transform> GetWhiteList_Copy() => whiteList.ToList();
        [SerializeField] private bool autoWhiteTheChild = true;//是否自动将白名单的子渲染器加入白名单
        public bool GetAutoWhiteTheChild() => autoWhiteTheChild;
        public bool SetAutoWhiteTheChild(bool autoWhiteTheChild) => this.autoWhiteTheChild = autoWhiteTheChild;
        [SerializeField] private bool ignoreParticleSystem = true;
        public bool IgnoreParticleSystem() => ignoreParticleSystem;
        public void SetIgnoreParticleSystem(bool ignore) => ignoreParticleSystem = ignore;
        void Start()
        {
            AddTheChildRendererInWhiteList();
            FreshMaterialShader();
        }

        private void AddTheChildRendererInWhiteList()
        {
            if (!autoWhiteTheChild) return;
            foreach (Transform rd in whiteList.ToList())
            {
                if (rd == null) continue;
                List<Transform> clRds = rd.GetComponentsInChildren<Transform>().ToList();
                foreach (Transform clRd in clRds.ToList())
                {
                    if (clRd == null) continue;
                    if (!whiteList.Contains(clRd))
                    {
                        whiteList.Add(clRd);
                    }
                }
            }
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

            //方式1：修改实例材质,不影响其他物体
            foreach (Renderer renderer in renderers)
            {
                if (ignoreParticleSystem && (renderer is ParticleSystemRenderer)) continue;//默认跳过粒子系统
                                                                                           //排除白名单的物体
                if ((bool)whiteList?.Contains(renderer?.transform)) continue;

                Material[] materials = renderer.materials; // 注意：这会创建材质副本
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].shader = targetShader;
                }
                renderer.materials = materials; //必须重新赋值回去！
            }
        }
    }
}