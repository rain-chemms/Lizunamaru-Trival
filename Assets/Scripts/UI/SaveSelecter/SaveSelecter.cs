using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//存档选择器
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(Animator))]
public class SaveSelecter : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool isDisplay;
    public bool IsDisplay { get => isDisplay; set => isDisplay = value; }

    [SerializeField] private ScrollRect savesView;
    public ScrollRect SavesView { get => savesView; }
    //只有三个玩家档案可供使用
    [SerializeField] private SaveArchive archive1;
    public SaveArchive Archive1 { get => archive1; }
    [SerializeField] private SaveArchive archive2;
    public SaveArchive Archive2 { get => archive2; }
    [SerializeField] private SaveArchive archive3;
    public SaveArchive Archive3 { get => archive3; }
    [SerializeField] private SaveConfirmDialog confirmDialog;
    public SaveConfirmDialog ConfirmDialog { get => confirmDialog; }
    [SerializeField] private RenameSaveDialog renameDialog;
    public RenameSaveDialog RenameDialog { get => renameDialog; }
    void OnEnable()
    {
        //自动获取子物体
        if(savesView == null) savesView = GetComponentsInChildren<ScrollRect>().Where(x => x.name.Equals("SavesView")).FirstOrDefault();
        if(archive1 == null) archive1 = GetComponentsInChildren<SaveArchive>().Where(x => x.name.Equals("Archive1")).FirstOrDefault();
        if(archive2 == null) archive2 = GetComponentsInChildren<SaveArchive>().Where(x => x.name.Equals("Archive2")).FirstOrDefault();
        if(archive3 == null) archive3 = GetComponentsInChildren<SaveArchive>().Where(x => x.name.Equals("Archive3")).FirstOrDefault();
        if(confirmDialog == null) confirmDialog = GetComponentInChildren<SaveConfirmDialog>();
        if(animator == null) animator = GetComponent<Animator>();
        archive1.Index = 0;
        archive2.Index = 1;
        archive3.Index = 2;
    }

    //游戏开始时动态读取存档数据
    void Start()
    {
        LoadTheDataInArchive();
    }

    private void LoadTheDataInArchive()
    {
        //检测当前存档
    }

    void Update()
    {
        //设置动画器
        animator?.SetBool("IsOpen",isDisplay);
    }
}
