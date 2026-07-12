using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BattleMessage))]
public class BattleRoundController : MonoBehaviour
{
    [SerializeField] private BattleMessage battleMessage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (battleMessage == null) battleMessage = GetComponent<BattleMessage>();
        //开始时是玩家回合
        BattleMessage.instance.SetIsPlayerTurn(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(_isChangingTurn) return;
        CheckRoundEnd();
    }
    private bool _isChangingTurn = false;
    private void CheckRoundEnd()
    {
        bool side = BattleMessage.instance.IsPlayerTurn();
        foreach (Role role in BattleMessage.instance.GetRoleList().ToList())
        {
            if (role.GetSide() == side)//是当前回合的玩家角色
            {
                if (!role.IsRoundOperateEnd()) return;
            }
        }
        _isChangingTurn = true;
        StartCoroutine(ChangeTurnSafe());
    }

    private IEnumerator ChangeTurnSafe()
    {
        if (BattleMessage.instance != null)
        {
            yield return BattleMessage.instance.ChangeTurn();
        }
        else
        {
            Debug.LogError("[BattleRoundController] BattleMessage.instance is null during turn change!");
        }
        
        //无论切回合是否成功,都必须解锁,否则游戏永久卡死
        _isChangingTurn = false;
        yield return null;
    }
}
