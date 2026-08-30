using UnityEngine;
using System;

namespace GridObjectSystem
{
    //这个脚本代表所有在战斗网格中物体的基类
    [RequireComponent(typeof(Rigidbody))]
    public class GridObject : MonoBehaviour
    {
        //棋盘上的物品是有阵营划分的
        [SerializeField] private bool side = true;//角色的阵营
        public void SetSide(bool nowSide) => side = nowSide;
        public bool GetSide() => side;
        //期盼物体必须要由刚体
        [SerializeField] protected Rigidbody rb;
        public Rigidbody GetRigidBody() => rb;
        protected virtual void OnEnable()
        {
            if(rb == null) rb = GetComponent<Rigidbody>();
            if(BattleBoard.instance != null) transform.SetParent(BattleBoard.instance?.transform);//挂载在棋盘上
        }

        void Start()
        {
            if(BattleBoard.instance != null) transform.SetParent(BattleBoard.instance?.transform);//挂载在棋盘上    
            lastDirection = direction;//缓存上次的朝向
        }

        //棋盘物体的方向
        [SerializeField] protected BattleDirection direction = BattleDirection.RIGHT;//角色的朝向
        public BattleDirection GetDirection() => direction;
        public void SetDirection(BattleDirection direction) => this.direction = direction;
        //物体位置移动相关的属性
        [SerializeField] protected float speed = 5.0f;//角色当前的速度
        public float GetSpeed() => speed;
        public void SetSpeed(float nowSpeed) => speed = nowSpeed;
        [SerializeField] protected bool isFly = false;//是否正在飞行
        public bool IsFly() => isFly;
        public void SetFly(bool isFly) => this.isFly = isFly;
        [SerializeField] protected Vector2Int gridIndex = Vector2Int.zero;//玩家当前所处的棋盘格索引 
        public Vector2Int GetGridIndex() => new Vector2Int(gridIndex.x,gridIndex.y);
        public void SetGridIndex(Vector2Int index) => gridIndex = index;
        public void SetGridIndex(int x,int y) => gridIndex = new Vector2Int(x,y);

        //角色委托Action事件
        //方向变化Action
        public event Action directionChangeAction = null;//角色朝向发生变化时调用的委托
        public Action GetDirectionChangeAction() => directionChangeAction;
        [NonSerialized] private BattleDirection lastDirection = BattleDirection.RIGHT;
        private void CheckDirectionChange()
        {
            if(direction != lastDirection)
            {
                Debug.Log("Action numbers: " + directionChangeAction.GetInvocationList().Length);
                directionChangeAction?.Invoke();//角色朝向发生变化激活委托
                lastDirection = direction;
            }
        }
        //内部控制器,用于事件检测
        protected virtual void Update()
        {
            CheckDirectionChange();
        }
    }    
}