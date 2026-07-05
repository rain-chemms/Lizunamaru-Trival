using UnityEngine;

public class ConcentratePoint : MonoBehaviour
{
    public static ConcentratePoint instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    [SerializeField] private bool isLocked = false;
    public bool IsLocked()
    {
        return isLocked;
    }

    public void SetIsLocked(bool _lock)
    {
        isLocked = _lock;
    }
    
    [SerializeField] private bool isDisplay = false;
    public bool IsDisplay()
    {
        return isDisplay;
    }
    public void SetDisplay(bool display)
    {
        isDisplay = display;
    }
    [SerializeField] private Vector2Int index = Vector2Int.zero;//当前聚焦的格子索引
    public Vector2Int GetIndex()
    {
        return index;
    }
    public void SetIndex(Vector2Int index)
    {
        this.index = index;
    }
}
