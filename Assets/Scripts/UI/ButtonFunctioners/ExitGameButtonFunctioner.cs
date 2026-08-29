using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ExitGameButtonFunctioner : MonoBehaviour
{
    [SerializeField] private Button button;
    void OnEnable()
    {
        if (button == null) button = GetComponent<Button>();
        button?.onClick.AddListener(ExitGame);
    }

    void OnDisable()
    {
        button?.onClick.RemoveListener(ExitGame);
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        // 编辑器下仅停止 Play Mode，防止意外关闭编辑器
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("[QuitGame] 编辑器环境：已停止播放模式");
#else
        // 真机 / 打包环境下正常退出
        Application.Quit();
        Debug.Log("[QuitGame] 正在退出应用...");
#endif
    }
}
