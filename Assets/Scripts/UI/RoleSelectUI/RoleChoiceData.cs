using System.Collections.Generic;
using GridObjectSystem.RoleSystem;
using UnityEngine;

namespace RoleChoiceUISystem
{
    //玩家选择的角色的数据信息
    [CreateAssetMenu(fileName = "RoleChoiceData", menuName = "RoleChoiceUISystem/RoleChoiceData", order = 0)]
    public class RoleChoiceData : ScriptableObject
    {
        [SerializeField] private string roleName;//角色的名字
        public string RoleName {get => roleName;}
        [SerializeField] private Role rolePrefab;//角色的预制体
        public Role RolePrefab {get => rolePrefab;}
        [SerializeField] private ProfileArt rolePrefileArt;//角色的立绘
        public ProfileArt RoleProfileArt {get => rolePrefileArt;}
        [SerializeField] private List<string> useCardPool;//角色使用的卡池
        public List<string> UseCardPool {get => useCardPool;}
        //初始数值
        [SerializeField] private uint startCoin;//初始金币
        public uint StartCoin {get => startCoin;}
        [SerializeField] private float startMaxHp;//初始血量
        public float StartMaxHp {get => startMaxHp;}
        [SerializeField] private string roleDiscribeTableName = "RoleDiscribe";//角色描述文本的数据库名字
        public string RoleDiscribeTableName {get => roleDiscribeTableName;}
        [SerializeField] private string roleDiscribeKey = "RoleDiscribe_Debug";//角色描述在本地化系统中的键值
        public string RoleDiscribeKey {get => roleDiscribeKey;}
    }
}
