using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GridObjectSystem.RoleSystem;
using GridObjectSystem.AbilitySystem;
using System.Collections.Generic;
using System.Linq;
using System;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
public class RoleMessageDisplayer : MonoBehaviour
{
    /*
        基础信息相关
    */
    [Header("基础信息")]
    [SerializeField] private float lerpSpeed = 2.0f;
    public void SetLerpTime(float lerpSpeed) => this.lerpSpeed = lerpSpeed;
    public float GetLerpTime() => lerpSpeed;

    [SerializeField] private Role role;
    public Role GetRole() => role;
    public void SetRole(Role role) => this.role = role;

    [SerializeField] private Canvas canvas;
    
    void OnEnable()
    {
        if(canvas == null) canvas = GetComponent<Canvas>();
        if(defendAnimator == null) defendAnimator = GetComponentsInChildren<Animator>().Where(x => x.gameObject.name.Equals("Defend")).FirstOrDefault();
        //尝试获取角色防御点数
        lastDefendPoint = (uint)role?.GetDefend();
        if(role == null) role = GetComponentInParent<Role>();
    }

    [SerializeField] private bool isDisplay = true;
    public bool IsDisplay() => isDisplay;
    public void SetDisplay(bool isDisplay) => this.isDisplay = isDisplay;
    
    void Update()
    {
        CheckDisplayState();
        CheckHp();
        CheckDefend();
        CheckAbility();
    }

    private void CheckDisplayState() => canvas.enabled = isDisplay;
    /*
        角色生命值相关的UI部件
    */
    [Header("角色生命值相关")]
    [SerializeField] private Canvas hpCanvas;
    public Canvas GetHpCanvas() => hpCanvas;
    
    [SerializeField] private TMP_Text hpText;//血量文本
    public TMP_Text GetHpText() => hpText;
    
    [SerializeField] private Slider hpLerp;//数值渐变条
    public Slider GetHpLerp() => hpLerp;
    [SerializeField] private Slider hpInstant;//实时瞬变条
    public Slider GetHpInstant() => hpInstant;

    private void CheckHp()
    {
        if(role == null || hpInstant == null || hpLerp == null || hpText == null) return;
        //设置血量文本
        hpText.text = role.GetMaxHp().ToString("0.0") + "/" +  role.GetHp().ToString("0.0");
        //设置血量渐变条
        hpLerp.maxValue = role.GetMaxHp();
        hpLerp.value = Mathf.Lerp(hpLerp.value, role.GetHp(), Time.deltaTime * lerpSpeed);
        if(hpLerp.value <= 0.01f) hpLerp.value = 0.0f;
        //设置血量实时瞬变条
        hpInstant.maxValue = role.GetMaxHp();
        hpInstant.value = role.GetHp();
    }
    /*
        角色防御相关UI部件
    */
    [Header("角色防御相关")]
    [SerializeField] private Canvas defendCanvas;
    public Canvas GetDefendCanvas() => defendCanvas;
    [SerializeField] private TMP_Text defendText;
    public TMP_Text GetDefendText() => defendText;
    
    [SerializeField] private Animator defendAnimator;
    
    [NonSerialized] private float lastDefendPoint;//上一次的防御点数
    private void CheckDefend()
    {
        if(role == null || defendText == null) return;
        //获取当前防御点数
        uint nowDefendPoint = (uint)role?.GetDefend();
        //触发动画器
        if(nowDefendPoint > lastDefendPoint)
        {
            defendAnimator?.SetTrigger("GainDefend");
        }
        else if(nowDefendPoint <= 0 && lastDefendPoint > 0)//破防了
        {
            defendAnimator?.SetTrigger("Cracked");
        }
        else if(nowDefendPoint < lastDefendPoint)//防御降低了
        {
            defendAnimator?.SetTrigger("LoseDefend");
        }
        //设置防御点数文字显示
        defendText.text = nowDefendPoint.ToString();
        lastDefendPoint = nowDefendPoint;//保存当前防御点数
    }

    /*
        角色能力系统显示相关
    */
    [Header("角色能力系统显示相关")]
    [SerializeField] private Canvas abilityBar;//能力栏
    //单个能力显示的预制体
    [SerializeField] private AbilityIcon abilityIconPrefab;

    [NonSerialized] private Dictionary<Ability,AbilityIcon> abtIconDict = new Dictionary<Ability,AbilityIcon>();//能力图标列表,用于管理显示
    //检测角色的能力列表
    //某些能力可能还会和其他能力有关,因此需要在预制体中写明
    private void CheckAbility()
    {
        Dictionary<Ability,int> abilityDict = role?.GetAbilityDict();
        //检查abtIconDict中是否有角色不存在的ability
        List<Ability> roleAbtList = abilityDict.Keys.ToList();
        foreach(KeyValuePair<Ability,AbilityIcon> icon in abtIconDict.ToList())
        {
            Ability abt = icon.Key;
            //角色不存在该能力,则销毁图标
            if(abt == null || !roleAbtList.Contains(abt))
            {
                Destroy(icon.Value.gameObject);
                abtIconDict.Remove(abt);
            }
        }
        //刷新键值对显示
        foreach(KeyValuePair<Ability,int> ability in abilityDict.ToList())
        {
            Ability abt = ability.Key;
            int layer = ability.Value;
            //若存在该能力,则刷新能力层数
            if(abtIconDict.ContainsKey(abt))
            {
                AbilityIcon icon = abtIconDict[abt];
                icon.RefreshLayerDisplay(layer,abt);
            }
            else//不存在能力则创建能力图标
            {
                AbilityIcon icon = Instantiate(abilityIconPrefab,abilityBar.transform);//设置其父物体为abilityBar
                icon.SetIconDisplay(abt,layer);
                abtIconDict.Add(abt,icon);//添加键值对
            }
        }
    }

}   
