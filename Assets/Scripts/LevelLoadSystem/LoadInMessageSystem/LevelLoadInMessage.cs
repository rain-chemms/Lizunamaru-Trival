using MapSystem;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace LevelLoadSystem.LoadInMessageSystem
{
    public abstract class LevelLoadInMessage : ScriptableObject
    {
        //关卡的类别
        [Header("关卡类别:当前关卡是什么类型")]
        [SerializeField] protected MapNodeCategory category;
        public MapNodeCategory GetCategory() => category;
        [Header("区域过滤器:不会在过滤的区域中加载")]
        [SerializeField] protected List<MapAreaCategory> areaFilter;//不会在哪一个区域中中出现
        public List<MapAreaCategory> GetAreaFilter() => areaFilter;
        [Header("层数限制:防止玩家前期遇到较强的敌人")]
        [SerializeField] protected int layerRestrict = -1;//玩家所处当前区域层数高于当前值的时候才能加载到.<= 0 代表不限制层数加载
        public int GetLayerRestrict() => layerRestrict;
        protected LevelLoadInMessage()
        {}
    }
}