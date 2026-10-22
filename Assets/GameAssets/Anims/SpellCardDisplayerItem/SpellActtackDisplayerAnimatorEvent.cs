using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(SpellAttackDisplayItem))]
public class SpellActtackDisplayItemAnimatorEvent : MonoBehaviour
{
    [SerializeField] private SpellAttackDisplayItem item;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        if(item == null) item = GetComponent<SpellAttackDisplayItem>();
    }

    //播放某一个符卡释放的音频    
    public void PlayTheSpellAttackVoice(string audioName)
    {
        if(item == null) return;
        foreach (AudioSource aS in SpellAttackDisplayer.instance?.GetAudioSources()?.ToList())
        {
            if(aS == null) continue;
            if (aS.name.Equals(audioName))
            {
                aS.Play();
            }
        }
    }

    public void RemoveSelfFormItemListAndDestroyItem()
    {
        var lList = SpellAttackDisplayer.instance?.LeftList;
        var rList = SpellAttackDisplayer.instance?.RightList;
        if(lList.Contains(item)) lList.Remove(item);
        if(rList.Contains(item)) rList.Remove(item);
        Destroy(item?.gameObject);
    }
}
