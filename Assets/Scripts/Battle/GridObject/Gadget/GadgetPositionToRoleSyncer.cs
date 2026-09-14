/// <summary>
/// 道具角色位置同步器
/// 挂载这个脚本后会依据道具关联的玩家的位置信息自动同步道具位置
/// </summary>
using UnityEngine;
using GridObjectSystem;

namespace GridObjectSystem.GadgetSystem
{
    [RequireComponent(typeof(Gadget))]
    public class GadgetPositionToRoleSyncer : MonoBehaviour
    {
        [SerializeField] private Gadget gadget;
        void OnEnable()
        {
            if(gadget == null) gadget = GetComponent<Gadget>();
            isDirSyncOpen = true;
            isPosSyncOpen = true;
        }
        [SerializeField] private bool isPosSyncOpen = true;//是否开启位置同步
        public bool IsPosSyncOpen() => isPosSyncOpen;
        public void SetPosSyncOpen(bool isOpen) => isPosSyncOpen = isOpen; 
        [Header("是否以玩家方向为基础坐标,启动后默认以BattleDirection.UP为主方向")]
        [SerializeField] private bool effectByRoleDirection = false;//是否受角色的朝向控制同步影响
        public bool IsEffectByRoleDirection() => effectByRoleDirection;
        public void SetEffectByRoleDirection(bool isEffect) => effectByRoleDirection = isEffect;
        [SerializeField] private bool isDirSyncOpen = true;//是否开启朝向同步
        public void SetDirSyncOpen(bool isOpen) => isDirSyncOpen = isOpen;
        public bool IsDirSyncOpen() => isDirSyncOpen;
        [SerializeField] private bool isFlySyncOpen = true;//是否开启飞行同步
        public bool IsFlySyncOpen() => isFlySyncOpen;
        public void SetFlySyncOpen(bool isOpen) => isFlySyncOpen = isOpen;
        [SerializeField] private bool revertFlyState = false;//是否反转飞行状态控制
        public bool IsFlyRevert() => revertFlyState; 
        public void SetRevertFlyState(bool isRevert) => revertFlyState = isRevert;
        [SerializeField] private Vector2Int gapsToRole;//道具与角色之间的坐标值间隔
        public Vector2Int GetGapsToRole() => gapsToRole;
        public void SetGapsToRole(Vector2Int gaps) => gapsToRole = gaps;
        public void SetGapsToRole(int x,int y) => gapsToRole = new Vector2Int(x,y);
        
        void Update()
        {
            SyncThePosition();
            SyncTheDirection();
            SyncTheFlyState();
        }
    
        private void SyncThePosition()
        {
            if(!isPosSyncOpen) return;
            if(gadget == null || gadget.GetBelongRole() == null) return;
            Vector2Int gTR = gapsToRole;
            GridObject role = gadget.GetBelongRole();
            if(effectByRoleDirection)
            {
                Vector2Int temp = Vector2Int.zero;
                BattleDirection dir = (BattleDirection)role?.GetDirection();
                switch (dir)
                {
                    case BattleDirection.DOWN:
                        temp.x = -gTR.x;
                        temp.y = -gTR.y;
                        break;
                    case BattleDirection.RIGHT:
                        temp.x = gTR.y;
                        temp.y = -gTR.x;
                        break;
                    case BattleDirection.LEFT:
                        temp.x = -gTR.y;
                        temp.y = gTR.x;
                        break;
                    case BattleDirection.UP:
                    default:
                        temp.x = gTR.x;
                        temp.y = gTR.y;
                        break;
                }
                gTR = temp;
            }
            gadget.SetGridIndex((Vector2Int)role?.GetGridIndex() + gTR);
        }

        private void SyncTheDirection()
        {
            if(!isDirSyncOpen) return;
            if(gadget == null || gadget.GetBelongRole() == null) return;
            gadget.SetDirection(gadget.GetBelongRole().GetDirection());
        }

        private void SyncTheFlyState()
        {
            if(!isFlySyncOpen) return;
            if(gadget == null || gadget.GetBelongRole() == null) return;
            bool fly = gadget.GetBelongRole().IsFly();
            if(revertFlyState) fly = !fly;
            gadget.SetFly(fly);
        }
    
    }
}