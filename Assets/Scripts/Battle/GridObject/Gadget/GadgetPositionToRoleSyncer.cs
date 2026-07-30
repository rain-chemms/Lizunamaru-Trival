/// <summary>
/// 道具角色位置同步器
/// 挂载这个脚本后会依据道具关联的玩家的位置信息自动同步道具位置
/// </summary>
using UnityEngine;

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
        [SerializeField] private bool isDirSyncOpen = true;//是否开启朝向同步
        public void SetDirSyncOpen(bool isOpen) => isDirSyncOpen = isOpen;
        public bool IsDirSyncOpen() => isDirSyncOpen;
        [SerializeField] private bool isFlySyncOpen = true;//是否开启飞行同步
        public bool IsFlySyncOpen() => isFlySyncOpen;
        public void SetFlySyncOpen(bool isOpen) => isFlySyncOpen = isOpen;
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
            gadget.SetGridIndex(gadget.GetBelongRole().GetGridIndex() + gapsToRole);
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
            gadget.SetFly(gadget.GetBelongRole().IsFly());
        }
    
    }
}