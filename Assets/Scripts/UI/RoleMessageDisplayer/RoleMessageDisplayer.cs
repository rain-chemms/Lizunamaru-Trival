using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Animator))]
public class RoleMessageDisplayer : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 2.0f;
    public void SetLerpTime(float lerpSpeed)
    {
        this.lerpSpeed = lerpSpeed;
    }
    public float GetLerpTime()
    {
        return lerpSpeed;
    }
    [SerializeField] private Role role;
    public Role GetRole()
    {
        return role;
    }
    public void SetRole(Role role)
    {
        this.role = role;
    }
    [SerializeField] private Canvas canvas;
    [SerializeField] private Animator animator;
    void Start()
    {
        if(canvas == null) canvas = GetComponent<Canvas>();
        if(animator == null) animator = GetComponent<Animator>();
    }
    [SerializeField] private bool isDisplay = true;
    public bool IsDisplay()
    {
        return isDisplay;
    }
    public void SetDisplay(bool isDisplay)
    {
        this.isDisplay = isDisplay;
    }
    // Update is called once per frame
    void Update()
    {
        CheckDisplayState();
        CheckHp();
        CheckDefend();
    }

    private void CheckDisplayState()
    {
        canvas.enabled = isDisplay;
    }

    /*
        角色生命值相关的UI部件
    */
    [SerializeField] private Canvas hpCanvas;
    public Canvas GetHpCanvas()
    {
        return hpCanvas;
    }
    [SerializeField] private TMP_Text hpText;//血量文本
    public TMP_Text GetHpText()
    {
        return hpText;
    }
    [SerializeField] private Slider hpLerp;//数值渐变条
    public Slider GetHpLerp()
    {
        return hpLerp;
    }
    [SerializeField] private Slider hpInstant;//实时瞬变条
    public Slider GetHpInstant()
    {
        return hpInstant;
    }

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
    [SerializeField] private Canvas defendCanvas;
    public Canvas GetDefendCanvas()
    {
        return defendCanvas;
    }
    [SerializeField] private TMP_Text defendText;
    public TMP_Text GetDefendText()
    {
        return defendText;
    }
    private void CheckDefend()
    {
        if(role == null || defendText == null) return;
        uint defendPoint = role.GetDefend();
        if(defendPoint > 0)
        {
            defendText.text = defendPoint.ToString();
            animator.SetBool("HiddenDefend",false);
        }
        else
        {
            animator.SetBool("HiddenDefend",true);
        }
    }
}   
