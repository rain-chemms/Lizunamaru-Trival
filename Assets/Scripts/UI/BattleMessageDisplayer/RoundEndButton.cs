using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class RoundEndButton : MonoBehaviour
{
    [SerializeField] private Button button;

    public void Update()
    {
        CheckControlPlayerAndSetSelfActivate();
    }

    private void CheckControlPlayerAndSetSelfActivate()
    {
        Role player = BattleMessage.instance?.GetControlPlayer();
        if(player == null || player.enabled == false) button.interactable = false;
        else if(player.IsRoundOperateEnd()) button.interactable = false;
        else button.interactable = true;
    }

    public void OnRoundEndButtonClick()
    {
        //结束当前控制的玩家的回合
        BattleMessage.instance?.GetControlPlayer()?.SetRoundOperateEnd(true);
    }
}
