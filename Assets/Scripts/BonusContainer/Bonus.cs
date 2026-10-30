using UnityEngine;
using System.Collections;

namespace BonusSystem
{
    //奖励生效器,用来触发奖励生效
    [RequireComponent(typeof(BonusItem))]
    public abstract class Bonus : MonoBehaviour, IBonusEffectiver
    {
        [SerializeField] private bool effectiveSuccess = false;//用来控制当前奖励项是否生效,生效后的奖励会从奖励列表中移除
        public bool EffectiveSuccess { get => effectiveSuccess; set => effectiveSuccess = value; }

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
    }
}
