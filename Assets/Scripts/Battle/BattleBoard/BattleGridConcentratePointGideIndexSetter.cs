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
    [SerializeField] private bool inSide = false;
    public void OnPointerExit(PointerEventData eventData)
    {
        if(battleGrid == null) return;
        Debug.Log("[BattleGridConcentratePointGideIndexSetter] PointerExit : <" + battleGrid?.GetIndex() + ">");
        inSide = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(battleGrid == null) return;
        Debug.Log("[BattleGridConcentratePointGideIndexSetter] PointerEnter : <" + battleGrid?.GetIndex() + ">");
        inSide = true;
        //集中点未锁定时可以进行移动
        if(!(bool)ConcentratePoint.instance?.IsLocked())
        {
            ConcentratePoint.instance?.SetIndex((Vector2Int)battleGrid?.GetIndex());//设置聚焦点索引
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if(battleGrid == null) return;
        Debug.Log("[BattleGridConcentratePointGideIndexSetter] PointerDown : <" + battleGrid?.GetIndex() + ">");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if(battleGrid == null) return;
        Debug.Log("[BattleGridConcentratePointGideIndexSetter] PointerUp : <" + battleGrid?.GetIndex() + ">");
        //ConcentratePoint.instance?.GetComponent<Animator>()?.SetBool("WillConfirm",false);//修改动画器属性
        if(inSide) ConcentratePoint.instance?.SetIsLocked(!(bool)ConcentratePoint.instance?.IsLocked());//修改动画器属性
    }
}
