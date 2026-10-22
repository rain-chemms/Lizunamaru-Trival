using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
public class SpellAttackDisplayer : MonoBehaviour
{
    public static SpellAttackDisplayer instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    
    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();
    public List<AudioSource> GetAudioSources() => audioSources;
    void OnEnable()
    {
        //自动获取全部的音频源
        List<AudioSource> AS = GetComponentsInChildren<AudioSource>().ToList();
        foreach (AudioSource aS in AS)
        {
            if(aS == null) continue;
            if(audioSources.Contains(aS)) continue;
            audioSources.Add(aS);
        }
        if(displayArea == null)
        {
            displayArea = GetComponentsInChildren<Canvas>().Where(x=>x.name.Equals("DisplayArea")).FirstOrDefault();
        }
    }

    /*
        附加功能: 触发一次符卡显示
        为了实现多个符卡的同时显示,需要实现一个控制的动画预制体
    */
    [SerializeField] private SpellAttackDisplayItem itemPrefab; 
    [SerializeField] private Canvas displayArea;
    public Canvas DisplayArea { get => displayArea; }

    [SerializeField] private Vector2 offsetUnit = new Vector2(0.05f,0.05f);
    public Vector2 OffsetUnit { get => offsetUnit; set => offsetUnit = value; }
    //临时实体存放列表
    [SerializeField] private List<SpellAttackDisplayItem> leftList = new List<SpellAttackDisplayItem>();
    public List<SpellAttackDisplayItem> LeftList { get => leftList; }
    [SerializeField] private List<SpellAttackDisplayItem> rightList = new List<SpellAttackDisplayItem>();
    public List<SpellAttackDisplayItem> RightList { get => rightList; }
    public IEnumerator WakeDisplayer(Sprite sprite,bool leftOrRight = true,string textKey = "Spell_Debug",string searchTable = "SpellDisplayTexts")
    {
        SpellAttackDisplayItem newItem = Instantiate(itemPrefab,displayArea.transform);//在下侧新建一个Item
        if(leftOrRight) leftList.Add(newItem);//加入对应的管理列表中
        else rightList.Add(newItem);
        //检查当前的列表并设置偏移:内部为栈结构
        List<SpellAttackDisplayItem> targetList = leftOrRight ? leftList : rightList;
        int count = targetList.Count;
        int index = 0;
        foreach(var itm in targetList?.ToList())
        {
            if(itm == null) continue;
            itm.Offset = (count - index - 1) * (leftOrRight ? offsetUnit : -offsetUnit);
            index ++;
        }

        Image img = newItem.RoleSpellImage;
        if (img != null)
        {
            img.sprite = sprite;
        }
        //设置文本
        yield return LocalizationSettings.InitializationOperation;
        string keyName = textKey;
        // 异步获取
        var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(searchTable, keyName);
        yield return operation;
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            TMP_Text txt = newItem.SpellText;
            if (txt != null)
            {
                txt.text = operation.Result;
            }
        }
        else
        {
            Debug.LogError($"[CardSpellAttackWaker]: Display the Keywords Failed: {operation.OperationException}");
        }
        //播放动画
        Animator animator = newItem.Animator;
        if (animator != null)
        {
            animator?.SetTrigger(leftOrRight ? "Left" : "Right");
            yield return null;
            AnimatorStateInfo stateInfo =(AnimatorStateInfo)animator?.GetCurrentAnimatorStateInfo(0);
            float length = stateInfo.length;
            yield return new WaitForSeconds(length / animator.speed); //等待动画播放完毕 
        }
    }
}
