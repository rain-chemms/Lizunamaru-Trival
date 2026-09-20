using UnityEngine;
using UnityEngine.UI;
using CardSystem;

[RequireComponent(typeof(CardCheckMask))]
public class CardCheckMaskMaterialSetter : MonoBehaviour
{
    [SerializeField] private CardCheckMask cardCheckMask;
    [SerializeField] private Image image;
    
    void OnEnable()
    {
        if(cardCheckMask == null) cardCheckMask = GetComponent<CardCheckMask>();
        if(image == null) image = GetComponent<Image>();
    }

    void Start()
    {
        SetTheMaterialToImage();
    }

    //依据卡牌的类型自动设置Image材质
    [SerializeField] private Material defaultMat;
    public Material GetDefaultMat() => defaultMat;
    
    [SerializeField] private SerializableDictionary<CardCategory, Material> categoryMatDict; 
    public SerializableDictionary<CardCategory, Material> GetCategoryMatDict() => categoryMatDict;
    
    private void SetTheMaterialToImage()
    {
        if(image == null) return;
        Card card = cardCheckMask?.GetCard();
        if(card != null)
        {
            CardCategory category = card.GetCardCategory();
            if(categoryMatDict.ContainsKey(category))
            {
                image.material = categoryMatDict[category];
            }
            else image.material = defaultMat;
        }
    }
}
