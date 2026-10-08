using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using GameSaveSystem;
using TMPro;

[RequireComponent(typeof(Canvas))]
public class SaveArchive : MonoBehaviour
{
    [SerializeField] private int index;//存档的索引
    public int Index { get => index; set => index = value; }    
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button saveSelectButton;
    [SerializeField] private Button renameButton;
    [SerializeField] private SaveSelecter selecter;
    [SerializeField] private TMP_Text archiveDiscribe;
    [SerializeField] private TMP_Text indexDisplayer;
    void OnEnable()
    {
        if(selecter == null) selecter = GetComponentInParent<SaveSelecter>();
        
        if(saveSelectButton == null) saveSelectButton = GetComponentsInChildren<Button>().Where(x => x.name.Equals("SaveSelectButton")).FirstOrDefault();
        saveSelectButton?.onClick.AddListener(OnClickSelectButton);
        
        if(deleteButton == null) deleteButton = GetComponentsInChildren<Button>().Where(x => x.name.Equals("DeleteButton")).FirstOrDefault();
        deleteButton?.onClick.AddListener(OnClickDeleteButton);
        
        if(renameButton == null) renameButton = GetComponentsInChildren<Button>().Where(x => x.name.Equals("RenameButton")).FirstOrDefault();
        renameButton?.onClick.AddListener(OnClickRenameButton);
    }

    void Start()
    {
        GetAndDisplayData();
        CheckSaveAndVisiableButtons();
    }

    //获取对应的索引数据并将其显示在archiveDiscribe上
    private void GetAndDisplayData()
    {
        indexDisplayer.text = index.ToString();
        if((bool)SaveSystem.instance?.SaveExist(index))
        {
            GameSave save = SaveSystem.instance?.GameSaves[index];
            archiveDiscribe.text = save?.saveName;
        }
        else
        {
            archiveDiscribe.text = "Empty Archive";
        }
    }

    void OnDisable()
    {
        saveSelectButton?.onClick.RemoveListener(OnClickSelectButton);
        deleteButton?.onClick.RemoveListener(OnClickDeleteButton);
        renameButton?.onClick.RemoveListener(OnClickRenameButton);
    }

    private void CheckSaveAndVisiableButtons()
    {
        if((bool)SaveSystem.instance?.SaveExist(index))
        {
            if(deleteButton != null) deleteButton.interactable = true;
            if(renameButton != null) renameButton.interactable = true;
        }
        else
        {
            if(deleteButton != null) deleteButton.interactable = false;
            if(renameButton != null) renameButton.interactable = false;    
        }
    }

    private void OnClickRenameButton()
    {
        RenameTheSave();
    }

    private void RenameTheSave()
    {
        RenameSaveDialog dialog = selecter?.RenameDialog;
        dialog?.Show(
            (string newName) =>
            {
                //检测当前的存档是否存在
                if(!(bool)SaveSystem.instance?.SaveExist(index)) 
                {
                    Debug.Log($"[SaveArchive]: {index} Save Archive <Save> File Not Exist, can't Excute Rename");
                    return;
                }
                SaveSystem.instance?.LoadFileData();//文件存在就尝试加载数据
                GameSave gameSave = SaveSystem.instance?.GameSaves[index];
                if(gameSave != null) gameSave.saveName = newName;
                SaveSystem.instance?.SaveDataToFile(index,false,false,true);//保存文件
                SaveSystem.instance?.LoadFileData();//文件存在就尝试加载数据
                CheckSaveAndVisiableButtons();//刷新显示
                GetAndDisplayData();
                Debug.Log($"[SaveArchive]: {index} Save Archive Rename Finished!");    
            },
            (string newName) =>
            {
                Debug.Log($"[SaveArchive]: {index} Save Archive Rename Canceled!");
            }
        );    
    }

    private void OnClickDeleteButton()
    {
        SaveConfirmDialog dialog = selecter?.ConfirmDialog;
        dialog?.Show(
            () =>
            {
                SaveSystem saveSystem = SaveSystem.instance;
                saveSystem.DeleteSave(index);
                saveSystem.LoadFileData();
                CheckSaveAndVisiableButtons();
                GetAndDisplayData();
                Debug.Log($"[SaveArchive]: {index} Save Archive is Deleted!");       
            },
            () =>
            {
                Debug.Log("[SaveArchive]: Save Archive Delete Canceled!");
            }
        );
    }

    private void OnClickSelectButton()
    {
        SaveSystem saveSystem = SaveSystem.instance;
        bool archiveIsInit = !(bool)saveSystem?.SaveExist(index);
        //尝试创建存档,已经创建的存档不会再次创建
        saveSystem?.CreateANewSave(index);
        CheckSaveAndVisiableButtons();//刷新显示
        GetAndDisplayData();
        //重新加载当前存档
        saveSystem?.LoadFileData();
        if(archiveIsInit)
        {
            RenameTheSave();
        }
        //设置活跃存档的索引
        if(saveSystem != null) SaveSystem.instance.NowSaveIndex = index;
        selecter.IsDisplay = false;
        Debug.Log($"[SaveArchive]: Create Or Load <{index}> Save Archive");
    }
}
