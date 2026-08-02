using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace VfxDisplaySystem
{
    //特效实体:VfxDisplayer依据实体的名字进行调用
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AnimTrigger))]
    public class VfxEntity : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        void OnEnable()
        {
            if(animator == null) animator = GetComponent<Animator>();
        }

        /// <summary>
        /// VFX实体,只有含有这个脚本才能被调用并通过动画器显示Vfx
        /// </summary>
        /// <param name="animWait">是否等待动画播放结束</param>
        /// <param name="addTime">额外的等待时间,可以为负数</param>
        /// <returns></returns>
        [SerializeField] private string vfxTriggerName = "VfxPlay";
        public string GetVfxTriggerName() => vfxTriggerName;
        public void SetVfxTriggerName(string tname) => vfxTriggerName = tname;
        public IEnumerator ShowVfx_WaitTime(bool animWait = false,float addTime = 1.0f)
        {
            if(animator == null) yield break; 
            animator?.SetTrigger(vfxTriggerName);
            yield return null;        
            float time = addTime;
            if(animWait)
            {
                AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
                time += info.length / info.speed;
            }
            yield return new WaitForSeconds(time);
        } 
    }

}