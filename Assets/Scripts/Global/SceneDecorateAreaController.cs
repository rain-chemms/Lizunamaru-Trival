using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MapSystem;
using System;

namespace GlobalSystem
{
    //用于读取当前的地形类别来选择性的开启装饰区域
    //控制的是场景中物体的显示与隐藏
    public class SceneDecorateAreaController : MonoBehaviour
    {
        [Serializable]
        internal struct DecorateContent
        {
            [SerializeField] public List<Transform> objList;
        }


        [Header("不分任何地形一定会显示的装饰物")]
        [SerializeField] private List<Transform> alwaysDisplayObj;
        public List<Transform> AlwaysDisplayObj { get => alwaysDisplayObj; }
        [Header("根据地形类别显示的装饰物")]
        [SerializeField] private SerializableDictionary<MapAreaCategory, DecorateContent> decorateAreaList = new SerializableDictionary<MapAreaCategory, DecorateContent>();
        public void CheckAndDisplayDecorate(MapAreaCategory categor)
        {
            //开启当前的物体
            if(decorateAreaList.ContainsKey(categor))
            {
                foreach(Transform obj in decorateAreaList[categor].objList.ToList())
                {
                    if(obj == null) continue;
                    obj.gameObject.SetActive(true);
                }
            }
            //关闭其他物体
            foreach(var obj in decorateAreaList.ToList())
            {
                if(obj.Key == categor) continue;
                foreach(Transform item in obj.Value.objList.ToList())
                {
                    if(item == null) continue;
                    item.gameObject.SetActive(false);
                }
            }
            //最后控制 alwaysDisplayObj 中的物体显示
            foreach(Transform obj in alwaysDisplayObj.ToList())
            {
                if(obj == null) continue;
                obj.gameObject.SetActive(true);
            }
        }

        void Start()
        {
            //开始时检测单例中的具体信息
            CheckAndDisplayDecorate((MapAreaCategory)Map.instance?.GetMapArea());
        }
    }
}