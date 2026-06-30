using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Card))]
public class UnderCardUIChecker : MonoBehaviour,
    IEndDragHandler, IDragHandler
{
    [SerializeField] private Card card;
    void Start()
    {
        if (card == null) card = GetComponent<Card>();
    }

    // Update is called once per frame
    public void OnDrag(PointerEventData eventData)
    {
        CheckUnderCardUI();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetUIBoarderDisplayerHidden(underCardUI, true);
    }

    [SerializeField] private RectTransform underCardUI;
    public void CheckUnderCardUI()
    {
        //获取卡牌图片下方的物体
        RaycastHit2D[] hit2Ds = Physics2D.RaycastAll(card.transform.position, Vector2.down, 0.1f);
        if(hit2Ds == null || hit2Ds.Length == 0)    
        {   
            SetUIBoarderDisplayerHidden(underCardUI, true);
            underCardUI = null;
        }
        
        foreach (var hit2D in hit2Ds)
        {
            RectTransform ui = hit2D.collider.GetComponent<RectTransform>();
            if (ui != null)
            {
                if (underCardUI != ui) SetUIBoarderDisplayerHidden(underCardUI, true);
                underCardUI = ui;
                SetUIBoarderDisplayerHidden(ui, false);
            }
        }
        
    }

    private void SetUIBoarderDisplayerHidden(RectTransform ui, bool hidden)
    {
        if(ui == null) return;
        if(card == null) return;
        BoarderCheckCardFliter fliter = ui.GetComponentInChildren<BoarderCheckCardFliter>();
        //只有在含有fliter的物体且fliter包含当前卡牌种类时才进行显示
        if(fliter == null) return;
        if(fliter.HaveCategory(card.GetCardCategory()))
        {
            foreach (var child in ui.GetComponentsInChildren<UIBoarderDisplayer>())
            {
                if (child != null) child.SetHideBoarder(hidden);
            }
        }
    }

}
