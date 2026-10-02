using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class PlayerMessagePanel : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    void OnEnable()
    {
        if (canvas == null) canvas = GetComponent<Canvas>();
        if (hpAnimator == null) hpAnimator = hpImage?.transform.parent?.GetComponent<Animator>();
        if (coinsAnimator == null) coinsAnimator = coinsImage?.transform.parent?.GetComponent<Animator>();
    }

    void Start()
    {
        //初始化游戏信息
        lastCoins = (uint)BattleMessage.instance?.GetCoins();
        lastMaxHp = (float)BattleMessage.instance?.GetControlPlayer()?.GetMaxHp();
        lastHp = (float)BattleMessage.instance?.GetControlPlayer()?.GetHp();
        //初始化外观设置
        if (hpText != null) hpText.text = lastHp.ToString() + "/" + lastMaxHp.ToString();
        CheckAndSetHpSpriteAndMaterial();
        if (coinsText != null) coinsText.text = lastCoins.ToString();
        displayCoins = lastCoins;
    }

    //相关的UI组件
    //以后可以继续扩展
    [Header("血量相关")]
    //血量图像
    [SerializeField] private Image hpImage;
    //低血量图像材质设置
    [SerializeField] private Sprite lowHpSprite;
    [SerializeField] private Material lowHpMaterial;
    //中血量图像材质设置
    [SerializeField] private Sprite midumHpSprite;
    [SerializeField] private Material midumHpMaterial;
    //高血量图像材质设置
    [SerializeField] private Sprite highHpSprite;
    [SerializeField] private Material highHpMaterial;
    //血量文字
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Animator hpAnimator;
    [Header("金币数相关")]
    //金币图像
    [SerializeField] private Image coinsImage;
    //金币文字
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private Animator coinsAnimator;
    [Header("角色头像相关")]
    //头像图片
    [SerializeField] private Image avatarImage;

    [SerializeField] private Animator avatarAnimator;

    void Update()
    {
        SyncHpMessage();
        SyncCoinsMessage();
    }

    [NonSerialized] private Sprite currentHpSprite;
    [NonSerialized] private Material currentHpMaterial;

    [NonSerialized] private float lastHp = 0.0f;
    [NonSerialized] private float lastMaxHp = 0.0f;
    private readonly StringBuilder hpSb = new StringBuilder(16);
    private void SyncHpMessage()
    {
        if (BattleMessage.instance?.GetControlPlayer() != null)
        {
            float nowMaxHp = (float)BattleMessage.instance?.GetControlPlayer()?.GetMaxHp();
            float nowHp = (float)BattleMessage.instance?.GetControlPlayer()?.GetHp();
            //检测maxHp和nowHp
            if (nowMaxHp == lastMaxHp && nowHp == lastHp) return;
            hpSb.Clear();
            hpSb.Append(nowHp).Append('/').Append(nowMaxHp);
            if (hpText != null) hpText.SetText(hpSb);//设置血量显示
            lastHp = nowHp;
            lastMaxHp = nowMaxHp;
            //依据当前血量选择材质
            CheckAndSetHpSpriteAndMaterial();
            //这里可以添加额外的动画器处理逻辑
            /*
                暂时未实现
            */
        }
    }


    private void CheckAndSetHpSpriteAndMaterial()
    {
        float precent = lastMaxHp > 0 ? lastHp / lastMaxHp : 1f;
        Material targetMat = highHpMaterial;
        Sprite targetSpt = highHpSprite;
        //以每三分之一作为界限
        if (precent < 1f / 3f)
        {
            targetMat = lowHpMaterial;
            targetSpt = lowHpSprite;
        }
        else if (precent < 2f / 3f)
        {
            targetMat = midumHpMaterial;
            targetSpt = midumHpSprite;
        }
        if (hpImage != null)
        {
            // 只在 Sprite/Material 真正变化时才赋值，避免打断 Canvas 合批
            if (targetSpt != null && targetSpt != currentHpSprite)
            {
                hpImage.sprite = targetSpt;
                currentHpSprite = targetSpt;
            }
            if (targetMat != currentHpMaterial)
            {
                hpImage.material = targetMat;
                currentHpMaterial = targetMat;
            }
        }
    }

    [NonSerialized] private uint lastCoins = 0;
    [NonSerialized] private uint displayCoins = 0; // 当前显示中的金币值
    [Range(2, 10)][SerializeField] private uint coinsChangeObsticle = 3;//显示金币变化时的阻力
    private void SyncCoinsMessage()
    {
        uint nowCoins = (uint)BattleMessage.instance?.GetCoins();
        if (displayCoins == lastCoins && lastCoins == nowCoins) return; // 无变化时直接跳过

        if (lastCoins != nowCoins)
        {
            // 目标值变了，触发一次动画
            if (coinsAnimator != null) coinsAnimator.SetTrigger("CoinsChange");
        }

        // 渐变逼近目标值
        if (nowCoins > lastCoins)
        {
            uint diff = nowCoins - lastCoins;
            uint addUnit = diff / coinsChangeObsticle;
            if (addUnit < 1) addUnit = 1;
            lastCoins += addUnit;
        }
        else if (lastCoins > nowCoins)
        {
            uint diff = lastCoins - nowCoins;
            uint subUnit = diff / coinsChangeObsticle;
            if (subUnit < 1) subUnit = 1;
            lastCoins -= subUnit;
        }

        // 只在显示值变化时才更新 UI
        if (lastCoins != displayCoins)
        {
            displayCoins = lastCoins;
            if (coinsText != null) coinsText.text = displayCoins.ToString();
        }
    }
}
