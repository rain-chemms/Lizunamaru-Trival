using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class BattleGridConcentratePointGideIndexSetter : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler

{
    [SerializeField] private BattleGrid battleGrid;
    public void SetBattleGrid(BattleGrid battleGrid)
    {
        this.battleGrid = battleGrid;
    }
    public BattleGrid GetBattleGrid()
    {
        return battleGrid;
    }

    // Update is called once per frame
    public void OnPointerExit(PointerEventData eventData)
    {
        if(battleGrid == null) return;
        Debug.Log("[BattleGridConcentratePointGideIndexSetter] PointerExit : <" + battleGrid?.GetIndex() + ">");
    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(battleGrid == null) return;
        Debug.Log("[BattleGridConcentratePointGideIndexSetter] PointerEnter : <" + battleGrid?.GetIndex() + ">");
        ConcentratePoint.instance?.SetIndex((Vector2)battleGrid?.GetIndex());//设置聚焦点索引
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if(battleGrid == null) return;
        Debug.Log("[BattleGridConcentratePointGideIndexSetter] PointerDown : <" + battleGrid?.GetIndex() + ">");
        ConcentratePoint.instance?.GetComponent<Animator>()?.SetBool("WillConfirm",true);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if(battleGrid == null) return;
        Debug.Log("[BattleGridConcentratePointGideIndexSetter] PointerUp : <" + battleGrid?.GetIndex() + ">");
        ConcentratePoint.instance?.GetComponent<Animator>()?.SetBool("WillConfirm",false);
    }
}
