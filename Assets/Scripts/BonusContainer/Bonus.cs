using UnityEngine;
using System.Collections;

namespace BonusSystem
{
    //奖励生效器,用来触发奖励生效
    [RequireComponent(typeof(BonusItem))]
    public abstract class Bonus : MonoBehaviour, IBonusEffectiver
    {
        [SerializeField] protected bool effectiveSuccess = false;//用来控制当前奖励项是否生效,生效后的奖励会从奖励列表中移除
        public bool EffectiveSuccess { get => effectiveSuccess; set => effectiveSuccess = value; }
        //一个物体上只能挂载一个奖励组件
        public abstract IEnumerator OnEffective();
        private void OnValidate()
        {
            EnsureSingleInstance();
        }

        private void Reset()
        {
            EnsureSingleInstance();
        }

        private void EnsureSingleInstance()
        {
            // 获取物体上所有继承自 Bonus 的组件
            Bonus[] bonuses = GetComponents<Bonus>();

            // 如果数量大于1，说明有冲突
            if (bonuses.Length > 1)
            {
                Debug.LogWarning($"物体 {gameObject.name} 上只能存在一个 Bonus 子类，正在移除旧的组件。");

                // 遍历并销毁其他子类（保留当前这个）
                foreach (Bonus bonus in bonuses)
                {
                    // 如果这个组件不是当前脚本实例，就销毁它
                    if (bonus != this)
                    {
#if UNITY_EDITOR
                        UnityEditor.Undo.DestroyObjectImmediate(bonus);
#else
                        Destroy(bonus);
#endif
                    }
                }
            }
        }

        //一下部分大都是由代码设置的,一般不在Inspector中设置
        //键值对用于从本地化系统中获取奖励描述
        [SerializeField] protected string discribeTable = "";
        public string DiscribeTable { get => discribeTable; set => discribeTable = value; }
        [SerializeField] protected string discribeKey = "";
        public string DiscribeKey { get => discribeKey; set => discribeKey = value; }
        //奖励的数量相关
        [SerializeField] protected uint bonusCount = 0;
        public uint BonusCount { get => bonusCount; set => bonusCount = value; }
        [SerializeField] protected bool displayCount = true;//是否显示奖励数量
        public bool DisplayCount { get => displayCount; set => displayCount = value; }
        //奖励的外观相关
        [SerializeField] protected Sprite previewSprite = null;
        public Sprite PreviewSprite { get => previewSprite; set => previewSprite = value; }
    }
}
