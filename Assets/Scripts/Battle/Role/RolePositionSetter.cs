using UnityEngine;

[RequireComponent(typeof(Role))]
public class RolePositionSetter : MonoBehaviour
{
    [SerializeField] private float flyHeight;//飞行时的高度
    public void SetFlyHeight(float flyHeight)
    {
        this.flyHeight = flyHeight;    
    }
    public float GetFlyHeight()
    {
        return flyHeight;
    }

    [SerializeField] private float grandYOffset;//地面高度Y的偏移量
    public void SetGrandYOffset(float offset)
    {
        grandYOffset = offset;
    }
    public float GetGrandYOffset()
    {
        return grandYOffset;
    }
    [SerializeField] private Role role;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //尝试自动获取
        if(role == null) role = GetComponent<Role>();
        SetRoleParentToBattleBoard();   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ChhangeLocalPositionByRoleData();
    }

    //将物体的父物体设置为战斗的棋盘
    public void SetRoleParentToBattleBoard()
    {
        if(role == null) return;
        role.GetRigidBody()?.transform.SetParent(BattleBoard.instance?.transform);
    }

    public void ChhangeLocalPositionByRoleData()
    {
        if(role == null) return;
        BattleBoard btb = BattleBoard.instance;
        if(btb == null) return;
        Rigidbody rb = role.GetRigidBody();
        if(rb == null) return;
        Vector2Int index = role.GetGridIndex();
        Vector3 _00Pos = btb.GetGrid00LocalPosition();
        Vector2 _gaps = btb.GetGapsOfGrid();
        bool isFly = role.IsFly();
        //实时计算role的相对位置
        float height = _00Pos.y + grandYOffset;
        float xPos = index.x * _gaps.x + _00Pos.x;
        float zPos = index.y * _gaps.y + _00Pos.z;
        if(isFly) height += flyHeight;
        //设置玩家位置
        rb.transform.localPosition = Vector3.Lerp(
            rb.transform.localPosition,
            new Vector3(xPos,height,zPos),
            role.GetSpeed() * Time.fixedDeltaTime
        );
    }
}
