using UnityEngine;
using System.Linq;

public class SeedSetter : MonoBehaviour
{
    public static SeedSetter instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private const uint hash = 5381;//种子的基数,写死的不能变
    [SerializeField] private string seed;
    public void SetSeed(string seed) => this.seed = seed;
    public string GetSeed() => seed;
    
    //产生一个随机的种子,并存储
    public void GenerateRandomSeed(int length = 8)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new System.Random();
        seed = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    //使用DJB2哈希算法将输入的字符串转换为一个64位整数
    //获取种子生成的随机数整数
    public int GetSeed_Int()
    {
        if (string.IsNullOrEmpty(seed)) return 0;
        uint hs = hash;
        foreach (char c in seed)
        {
            hs = ((hs << 5) + hs) + (uint)c;
        }
        return (int)hs;
    }

}
