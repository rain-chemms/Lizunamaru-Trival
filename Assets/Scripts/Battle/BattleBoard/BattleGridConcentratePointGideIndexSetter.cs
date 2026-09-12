using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class BattleGridConcentratePointGideIndexSetter : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler

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
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // 完整的点击：按下 + 抬起 都在同一物体上触发
        Debug.Log("[BattleGridConcentratePointGideIndexSetter] PointerClicked : <" + battleGrid?.GetIndex() + ">");
        if(inSide) ConcentratePoint.instance?.SetIsLocked(!(bool)ConcentratePoint.instance?.IsLocked());//交换锁定属性
    }
}
