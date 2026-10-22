using UnityEngine;
using UnityEngine.UI;
using TMPro;

//用于进行多次符卡显示的显示项
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Canvas))]
public class SpellAttackDisplayItem : MonoBehaviour
{
    [SerializeField] private TMP_Text spellText;
    public TMP_Text SpellText { get => spellText; }
    [SerializeField] private Image roleSpellImage;
    public Image RoleSpellImage { get => roleSpellImage; }
    [SerializeField] private Vector2 offset = Vector2.zero;
    public Vector2 Offset { get => offset; set => offset = value; }
    [SerializeField] private Animator animator;
    public Animator Animator { get => animator; }

    void OnEnable()
    {
        if(animator == null) animator = GetComponent<Animator>();
        if(roleSpellImage == null)
        {
            foreach(Image img in GetComponentsInChildren<Image>())
            {
                if(img == null) continue;
                if(img.name.Equals("RoleSpellImage")) 
                {
                    roleSpellImage = img;
                    break;
                }
            }
        }
        //尝试自动获取技能文本容器
        if(spellText == null)
        {
            foreach(TMP_Text txt in GetComponentsInChildren<TMP_Text>())
            {
                if(txt == null) continue;
                if(txt.name.Equals("SpellText"))
                {
                    spellText = txt;
                    break;
                }
            }
        }
    }

}