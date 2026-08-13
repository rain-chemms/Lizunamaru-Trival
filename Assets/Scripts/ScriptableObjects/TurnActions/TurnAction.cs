using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using BulletSystem;
using VfxDisplaySystem;

namespace GridObjectSystem.RoleSystem.AutoSystem
{
    public enum AttackCategory
    {
        DEFAULT,//默认攻击,设置的方向只于GridOffset有关
        RANDOM,//随机方向子弹
        SNIPE,//狙击弹
        DIRECTION,//方向子弹,朝着角色的面向的方向射击    
    }
    //回合内自动行为的基类,代表单个有效的行为
    [CreateAssetMenu(fileName = "InTurnAutoAction", menuName = "InTurnAutoAction/Actions/JustView(TurnAction)")]
    public class TurnAction : ScriptableObject
    {
        [Header("行为的基本信息")]
        [SerializeField] protected string actionName = "Bugs";//自动行为名称
        [SerializeField] protected bool isActionOpen = true;//是否开启
        [Header("要显示的Vfx列表")]
        [SerializeField] protected bool openVfx = false;
        [SerializeField] protected List<string> vfxList = new List<string>();//要显示的Vfx名称
        [Header("是否显示符卡界面")]
        [SerializeField] private bool openSpellDisplay = false;
        [SerializeField] private bool leftOrRightSD = false;//是在左侧还是右侧进行符卡的显示
        [SerializeField] private Sprite spellSprite;//符卡的人物立绘Sprite
        [SerializeField] private string spellTextKey = "Spell_Debug";//符卡的文本的本地化键值
        [SerializeField] private string searchTable = "SpellDisplayTexts";//搜索本地字典
        public virtual IEnumerator Excute(Role role)//行为执行函数,需要传入生效的玩家
        {
            if (openVfx)
            {
                foreach (string vfxName in vfxList)
                {
                    yield return VfxDisplayer.instance?.DisplayVfx(vfxName, false, 0.0f);
                }
            }
            if (openSpellDisplay) yield return SpellAttackDisplayer.instance?.WakeDisplayer(spellSprite, leftOrRightSD, spellTextKey, searchTable);
        }
    }
}