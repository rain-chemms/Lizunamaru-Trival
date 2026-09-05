using UnityEngine;
using GridObjectSystem.GadgetSystem;
using System.Collections.Generic;
using System.Collections;
using GridObjectSystem.GadgetSystem.Guns;
using GridObjectSystem.RoleSystem;
using System.Linq;


namespace CardSystem.AllCardHub
{
    // 插入卡槽时,在角色正前方召唤一个迷你八卦炉,不在卡槽中时移除,
    // 丢弃此牌时,激活一次本方的所"八卦炉"类(class of MiniHarkero)的道具
    public class Gadget_MiniHakerro : Card
    {
        [SerializeField] private MiniHakerro miniHarkerroPrefab;
        //弃牌的时候激活本方两个八卦炉之间的时间间隔
        [SerializeField] private float invokeHalt = 0.1f;
        public float GetInvokeHalt() => invokeHalt;
        //一张卡牌只能召唤一个八卦炉
        [SerializeField] private Gadget hakEntity;
        private void CreateMiniHakerro()
        {
            if (hakEntity != null) return;//已经存在八卦炉,则返回
            Role role = BattleMessage.instance?.GetControlPlayer();
            if (role == null) return;
            Gadget hak = Instantiate(miniHarkerroPrefab);
            //将八卦炉加入棋盘
            hak.transform.SetParent(BattleBoard.instance?.transform);
            //加入控制列表中
            if (!(bool)BattleMessage.instance?.GetGadgetList()?.Contains(hak)) BattleMessage.instance?.GetGadgetList()?.Add(hak);
            hakEntity = hak;
            //设置八卦炉初始位置
            hak.transform.position = (Vector3)role?.transform.position;
            //设置八卦炉的归属玩家
            hak.SetBelongRole(role);
            //设置八卦炉玩家位置同步器
            GadgetPositionToRoleSyncer bSyncer = hak.GetComponent<GadgetPositionToRoleSyncer>();
            bSyncer?.SetPosSyncOpen(true);
            bSyncer?.SetDirSyncOpen(true);
            bSyncer?.SetFlySyncOpen(true);
            //依据已有属于自己的八卦炉设置其偏移位置
            List<Gadget> hakList = BattleMessage.instance?.GetGadgetList()
                ?.FindAll(x =>
                    (x as MiniHakerro) != null && x.GetBelongRole() == role
                );
            //获取所有八卦炉的位置
            int offset = 1;
            int dir = 0;//代表四个方向:顺序 (y+:上) -> (y-:下) -> (x+:右) -> (x-:左)\
            Vector2Int hakOffset = Vector2Int.zero;
            int limt = 0;
            while (limt < int.MaxValue)//循环次限制,防止死循环
            {
                //获取目标方向最大值
                int cmpMax = 0;
                switch (dir)
                {
                    case 1: // 下 y-
                    case 3: // 左 x-
                        cmpMax = int.MaxValue; // Min操作需要极大初始值
                        break;
                    case 2: // 右 x+
                    case 0: // 上 y+
                    default:
                        cmpMax = int.MinValue; // Max操作需要极小初始值
                        break;
                }
                foreach (Gadget h in hakList)
                {
                    GadgetPositionToRoleSyncer syncer = h?.GetComponent<GadgetPositionToRoleSyncer>();
                    if (syncer == null) continue;
                    switch (dir)
                    {
                        case 2://右, 比较 x+
                            cmpMax = Mathf.Max(cmpMax, syncer.GetGapsToRole().x);
                            break;
                        case 1://下, 比较 y-
                            cmpMax = Mathf.Min(cmpMax, syncer.GetGapsToRole().y);
                            break;
                        case 3://左,比较 x-
                            cmpMax = Mathf.Min(cmpMax, syncer.GetGapsToRole().x);
                            break;
                        case 0://上,比较 y+
                        default:
                            cmpMax = Mathf.Max(cmpMax, syncer.GetGapsToRole().y);
                            break;
                    }
                }
                //比较最值
                bool canInsert = false;
                switch (dir)
                {
                    case 1://下,比较 y-            
                    case 3://左,比较 x-        
                        canInsert = cmpMax > -offset;
                        break;
                    case 2://右,比较 x+
                    case 0://上,比较 y+
                    default:
                        canInsert = cmpMax < offset;
                        break;
                }
                //当前可插入
                if (canInsert)
                {
                    //设置偏移量
                    switch (dir)
                    {
                        case 1://下,比较 y-
                            hakOffset = new Vector2Int(0, -offset);
                            break;
                        case 3://左,比较 x-        
                            hakOffset = new Vector2Int(-offset, 0);
                            break;
                        case 2://右,比较 x+
                            hakOffset = new Vector2Int(offset, 0);
                            break;
                        case 0://上,比较 y+
                        default:
                            hakOffset = new Vector2Int(0, offset);
                            break;
                    }
                    break;
                }

                dir += 1;
                if (dir % 4 == 0) offset += 1;//每循环完一次,偏移量加1
                dir %= 4;
                limt += 1;
            }
            bSyncer?.SetGapsToRole(hakOffset);
        }

        private IEnumerator DestroyHakEntity()
        {
            if (hakEntity == null) yield break;//不存在八卦炉,则返回
            Gadget gt = hakEntity;
            hakEntity = null;
            //等待动画播放完毕
            gt.GetComponent<AnimTrigger>()?.SetBoolValue("Open", false);
            yield return null;//暂停一帧
            AnimatorStateInfo info = (AnimatorStateInfo)gt.GetComponent<Animator>()?.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(info.length / info.speed);//等待动画播放完毕,固定数值大约时0.15
            //将这个道具从BattleMessage中删除
            BattleMessage.instance?.GetGadgetList()?.Remove(gt);
            //销毁这个道具
            Destroy(gt.gameObject);
        }

        void OnDestroy()
        {
            //销毁八卦炉
            //StartCoroutine(DestoryHakEntity());
            if (hakEntity != null)
            {
                BattleMessage.instance?.GetGadgetList()?.Remove(hakEntity);
                Destroy(hakEntity.gameObject);
                hakEntity = null;
            }
        }


        public override IEnumerator AfterInsertToSolt()
        {
            CreateMiniHakerro();
            return base.AfterInsertToSolt();
        }

        public override IEnumerator AfterRemoveFromSolt()
        {
            yield return DestroyHakEntity();
            yield return base.AfterRemoveFromSolt();
        }

        public override IEnumerator AfterDiscard()
        {
            bool yourSide = (bool)BattleMessage.instance?.GetControlPlayer()?.GetSide();
            MiniHakerro lastHak = null;
            foreach (Gadget gd in BattleMessage.instance?.GetGadgetList()?.ToList())
            {
                MiniHakerro hak = gd as MiniHakerro;
                if (hak!=null && hak.GetSide() == yourSide)
                {
                    yield return hak.OnGadgetEffect();//激活所有同阵营的八卦炉
                    lastHak = hak;
                    yield return new WaitForSeconds(invokeHalt);
                }
            }
            //等待最后一个八卦炉的动画播放完毕
            Animator animator = lastHak?.GetComponent<Animator>();
            AnimatorStateInfo info = (AnimatorStateInfo)animator?.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(info.length / info.speed);//等待动画播放完毕            
            yield return base.AfterDiscard();
        }
    }
}
