using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace VfxDisplaySystem{
    [RequireComponent(typeof(Canvas))]
    public class VfxDisplayer : MonoBehaviour
    {
        //使用单例模式
        public static VfxDisplayer instance;
        void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        //激活时检查子物体
        void OnEnable()
        {
            CheckChildVfxEntity();
            CloseAllMask();
        }
        //非激活态是清空列表
        void OnDisable()
        {
            vfxEntityList.Clear();
        }

        private void CloseAllMask()
        {
            //关闭所有图片的射线检测
            List<Image> maskList = GetComponentsInChildren<Image>(true).ToList();
            foreach(Image img in maskList)
            {
                if(img == null) continue;
                //关闭所有图片的射线检测
                img.maskable = false;
                img.raycastTarget = false;
            }
            //设置Canvas
            List<Canvas> canvasList = GetComponentsInChildren<Canvas>(true).ToList();
            foreach(Canvas canvas in canvasList)
            {
                canvas.worldCamera = Camera.main;
            }
        }
        private void CheckChildVfxEntity()
        {
            List<VfxEntity> tempList = GetComponentsInChildren<VfxEntity>(true).ToList();
            if(tempList == null) return;
            foreach(VfxEntity entity in tempList)
            {
                if(entity == null) continue;
                if(!vfxEntityList.Contains(entity))
                { 
                    vfxEntityList.Add(entity);
                    entity.gameObject.SetActive(true);
                }
            }
        }

        [SerializeField] private List<VfxEntity> vfxEntityList = new List<VfxEntity>();//所有管理的VFX实体
        public List<VfxEntity> GetVfxEntityList() => vfxEntityList;
        public void AddVfxEntity(VfxEntity entity)
        {
            if(entity == null) return;
            if(!vfxEntityList.Contains(entity)) 
            {
                vfxEntityList.Add(entity);//确保Vfx实体在列表中唯一
                entity.gameObject.SetActive(true);
                //将Vfx实体的父物体设置为VfxDisplayer   
                entity.transform.SetParent(transform);
            }
        }
        public void RemoveVfxEntity(VfxEntity entity)
        {
            if(entity == null) return;
            if(vfxEntityList.Contains(entity)) 
            {
                vfxEntityList.Remove(entity);
                entity.gameObject.SetActive(false);//关闭Vfx实体
            }
        
        }
        /// <summary>
        /// 用于外界调用以显示VFX
        /// </summary>
        /// <param name="vfxName">要显示的Vfx实体,必须是挂载VfxEntity脚本的物体</param>
        /// <param name="waitAnim">是否等待动画播放结束</param>
        /// <param name="addTime">附加的等待时间</param>
        public IEnumerator DisplayVfx(string vfxName,bool waitAnim = false,float addTime = 0.0f)
        {
            VfxEntity target = null;
            //寻找为vfxName的子物体
            foreach(VfxEntity entity in vfxEntityList)
            {
                if(entity == null) continue;
                if(entity.name == vfxName)
                {
                    target = entity;
                    break;
                }
            }
            yield return target?.ShowVfx_WaitTime(waitAnim,addTime);
        }

        //测试代码
        IEnumerator Start()
        {   
            yield return new WaitForSeconds(5.0f);
            StartCoroutine(DisplayVfx("NuclearCaution"));
        }
    }
}