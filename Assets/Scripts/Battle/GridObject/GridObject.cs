using UnityEngine;
using System;
using GridObjectSystem.AbilitySystem;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Collections;

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

        protected virtual void Start()
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
        //棋盘物体拥有的能力字典
        //存储格式:能力种类,能力层数
        [NonSerialized] protected Dictionary<Ability,int> abilityDict = new Dictionary<Ability,int>();
        public Dictionary<Ability,int> GetAbilityDict() => abilityDict;//获取能力字典
        //为物体添加一种能力,添加的层数可以为负数
        public IEnumerator AddAbility<AbilityType>(int addLayer) where AbilityType : Ability,new()
        {
            bool haveAbility = false;
            foreach(KeyValuePair<Ability,int> kv in abilityDict.ToList())
            {
                Ability ability = kv.Key;
                int nowLayer = kv.Value;
                if(ability == null) continue;
                //若当前类型的能力已存在,则更新层数
                if(ability.GetType().Equals(typeof(AbilityType)))
                {
                    //存在当前能力
                    haveAbility = true;
                    Debug.Log("[GridObject]: "+ name.ToString() +" Add Exist Ability: " + typeof(AbilityType).ToString() +", Now Number:"+abilityDict[ability]);
                    //计算层数
                    int newLayer = nowLayer + addLayer;
                    if(!ability.CanNegative) newLayer = Mathf.Max(newLayer,0);//检测不能小于0
                    if(!ability.CanStack)//检测不可叠加
                    {
                        if(Math.Abs(newLayer) > 1)
                        {
                            if(newLayer < 0) newLayer = -1;
                            if(newLayer > 1) newLayer = 1;
                        }
                    }
                    if(newLayer == 0)//当前类型abilities已不存在时,移除该能力
                    {
                        //触发移除该能力时的效果
                        yield return ((IAbilityFunctioner)ability)?.AfterAbilityAmountChanged(this);
                        yield return ((IAbilityFunctioner)ability)?.AfterAbilityRemoved(this);
                        abilityDict.Remove(ability);
                        Debug.Log("[GridObject]: "+ name.ToString() +" Remove Ability: " + typeof(AbilityType).ToString() + " Because Now Number is 0!");
                        break;
                    }
                    //触发层数改变
                    yield return ((IAbilityFunctioner)ability)?.AfterAbilityAmountChanged(this);
                    abilityDict[ability] = newLayer;
                    break;
                }
            }
            if(!haveAbility)
            {
                Debug.Log("[GridObject]: "+ name.ToString() +" Add New Ability: " + typeof(AbilityType).ToString() +", Now Number:"+ addLayer);
                if(addLayer == 0) yield break;//层数为0时,不添加
                Ability abt = new AbilityType();
                yield return ((IAbilityFunctioner)abt)?.AfterAbilityAdded(this);
                yield return ((IAbilityFunctioner)abt)?.AfterAbilityAmountChanged(this);
                abilityDict.Add(abt,addLayer);
            }
        }
        //通过名称获取的重写方法
        public IEnumerator AddAbility(string abilityName,int addLayer)
        {
            bool haveAbility = false;
            foreach(KeyValuePair<Ability,int> kv in abilityDict.ToList())
            {
                Ability ability = kv.Key;
                int nowLayer = kv.Value;
                if(ability == null) continue;
                //若当前类型abilities已存在,则更新层数
                if(ability.AbilityName.Equals(abilityName))
                {
                    //存在当前能力
                    haveAbility = true;
                    Debug.Log("[GridObject]: "+ name.ToString() +" Add Exist Ability: " + abilityName +", Now Number:"+abilityDict[ability]);
                    //计算层数
                    int newLayer = nowLayer + addLayer;
                    if(!ability.CanNegative) newLayer = Mathf.Max(newLayer,0);//检测不能小于0
                    if(!ability.CanStack)//检测不可叠加
                    {
                        if(Math.Abs(newLayer) > 1)
                        {
                            if(newLayer < 0) newLayer = -1;
                            if(newLayer > 1) newLayer = 1;
                        }
                    }
                    if(newLayer == 0)//当前类型abilities已不存在时,移除该能力
                    {
                        //触发移除该能力时的效果
                        yield return ((IAbilityFunctioner)ability)?.AfterAbilityAmountChanged(this);
                        yield return ((IAbilityFunctioner)ability)?.AfterAbilityRemoved(this);
                        abilityDict.Remove(ability);
                        Debug.Log("[GridObject]: "+ name.ToString() +" Remove Ability: " + abilityName + " Because Now Number is 0!");
                        break;
                    }
                    //触发层数改变
                    yield return ((IAbilityFunctioner)ability).AfterAbilityAmountChanged(this);
                    abilityDict[ability] = newLayer;
                    break;
                }
                Debug.Log("[GridObject]: "+ name.ToString() +" Add Exist Ability: " + abilityName +", Now Number:"+abilityDict[ability]);
                
            }    
            if(!haveAbility)
            {
                Debug.Log("[GridObject]: "+ name.ToString() +" Add New Ability: " + abilityName +", Now Number:"+ addLayer);
                if(addLayer == 0) yield break;
                // 只知道类名,尝试获取类的对象
                // 需要搜索程序集
                Type type = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(t => t.Name == abilityName && typeof(Ability).IsAssignableFrom(t));
                //程序集中存在类该的定义
                if(type != null)
                {
                    Ability abt = (Ability)Activator.CreateInstance(type);//创建能力实例
                    yield return ((IAbilityFunctioner)abt)?.AfterAbilityAdded(this);
                    yield return ((IAbilityFunctioner)abt)?.AfterAbilityAmountChanged(this);
                    abilityDict.Add(abt,addLayer);//添加该能力
                }
            }
        }

        //棋盘物体委托Action事件
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