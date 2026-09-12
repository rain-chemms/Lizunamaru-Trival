using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

//用于设置是否显示能力的描述文字
[RequireComponent(typeof(AbilityIcon))]
public class AbilityDiscribeChecker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform abilityDiscription;//描述文字对应的游戏物体
    public Transform AbilityDiscription
    {
        get { return abilityDiscription; }
        set { abilityDiscription = value; }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        abilityDiscription.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        abilityDiscription.gameObject.SetActive(false);
    }
}