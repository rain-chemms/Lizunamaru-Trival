using UnityEngine;

[RequireComponent(typeof(Card))]
public class SpellCardDisplaySpriteSetter : MonoBehaviour
{
    [SerializeField] private string spellTocalTextKey = "Spell_Debug";//本地化文本的键值
    public string GetSpellTocalTextKey()
    {
        return spellTocalTextKey;
    }
    public void SetSpellTocalTextKey(string newSpellTocalTextKey)
    {
        spellTocalTextKey = newSpellTocalTextKey;
    }
    [SerializeField] private Sprite spellSprite;
    public Sprite GetSpellSprite()
    {
        return spellSprite;
    }
    public void SetSpellSprite(Sprite newSpellSprite)
    {
        spellSprite = newSpellSprite;
    }
}