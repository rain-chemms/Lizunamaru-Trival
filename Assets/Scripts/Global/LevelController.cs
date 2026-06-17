using UnityEngine;

//关卡控制器
//控制有关战斗内关卡数据的加载,角色敌人的初始化
//采用单例模式
public class LevelController : MonoBehaviour
{
    public static LevelController instance;
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
}
