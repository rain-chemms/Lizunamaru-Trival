using UnityEngine;

public class SeedSetter : MonoBehaviour
{
    public static SeedSetter instance;
    void  Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    private const uint hash = 5381;//种子的基数,写死的不能变
    [SerializeField] private string seed;
    public void SetSeed(string seed)
    {
        this.seed = seed;
    }
    public string GetSeed()
    {
        return seed;
    }

    //使用DJB2哈希算法将输入的字符串转换为一个64位整数
    //获取种子生成的随机数整数
    public int GetSeed_Int()
    {
        if(string.IsNullOrEmpty(seed)) return 0;
        uint hs = hash;
        foreach(char c in seed)
        {
            hs = ((hs<<5)+hs) + (uint)c;
        }
        return (int)hs;
    } 

}
