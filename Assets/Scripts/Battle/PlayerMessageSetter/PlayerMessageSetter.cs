using UnityEngine;

//玩家信息获取器
/*
    用于获取玩家的所有信息
    包括:
        卡牌索引列表:有什么卡
        当前选择的角色对应的Role预制体/或其索引列表:选择的角色是谁
        金币数量
        剩余生命值
        最大生命值
        +角色当前所处地图区域的第几层,正在哪个地图区域(第几面)==>(读取这个信息 + 种子生成器的信息 => 复现地图Map信息)
        +角色的其他信息
*/
public class PlayerMessageSetter : MonoBehaviour
{
    public static PlayerMessageSetter instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
