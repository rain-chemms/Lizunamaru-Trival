using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoleChoiceUISystem
{
    [RequireComponent(typeof(Animator))]
    public class RoleChoiceUI : MonoBehaviour
    {
        //动画控制相关
        [SerializeField] private bool isDisplay = false;
        public bool IsDisplay {get => isDisplay; set => isDisplay = value;}
        [SerializeField] private Animator animator;

        [SerializeField] private Transform profileArtArea;//角色立绘显示区域
        public Transform ProfileArtArea { get => profileArtArea; }
        [SerializeField] private ScrollRect choiceBar;//选择按钮区域
        public ScrollRect ChoiceBar { get => choiceBar; }
        [SerializeField] private TMP_Text roleDiscribeArea;//角色描述文字
        public TMP_Text RoleDiscribeArea { get => roleDiscribeArea; }
        [SerializeField] private TMP_Text hpText;
        public TMP_Text HpText {get => hpText; }
        [SerializeField] private TMP_Text startCoinText;
        public TMP_Text StartCoinText {get => startCoinText; }
        [SerializeField] private TMP_Text nameText;
        public TMP_Text NameText {get => nameText; }
        [SerializeField] private Button newGameStartButton;
        public Button NewGameStartButton { get => newGameStartButton; }
        
        void OnEnable()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (profileArtArea == null) profileArtArea = transform.Find("ProfileArtArea");
            if (choiceBar == null) choiceBar = GetComponentsInChildren<ScrollRect>().Where(x => x.name.Equals("ChoiceBar")).FirstOrDefault();
            if (roleDiscribeArea == null) roleDiscribeArea = GetComponentsInChildren<TMP_Text>().Where(x => x.name.Equals("RoleDiscribe")).FirstOrDefault();
            if (hpText == null) hpText = GetComponentsInChildren<TMP_Text>().Where(x => x.name.Equals("StartHp")).FirstOrDefault();
            if (startCoinText == null) startCoinText = GetComponentsInChildren<TMP_Text>().Where(x => x.name.Equals("StartCoin")).FirstOrDefault();
            if (nameText == null) nameText = GetComponentsInChildren<TMP_Text>().Where(x => x.name.Equals("RoleName")).FirstOrDefault();
            if (newGameStartButton == null) newGameStartButton = GetComponentsInChildren<Button>().Where(x => x.name.Equals("NewGameStart")).FirstOrDefault();
            
            //清空对应区域内容区域
            if (profileArtArea != null)
            {
                foreach (Transform child in profileArtArea)
                {
                    Destroy(child.gameObject);
                }
            }
            if(roleDiscribeArea != null) roleDiscribeArea.text = "";
        }

        void Update()
        {
            animator?.SetBool("IsOpen",isDisplay);
        }
    }
}
