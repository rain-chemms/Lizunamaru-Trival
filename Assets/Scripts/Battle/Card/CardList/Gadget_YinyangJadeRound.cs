using UnityEngine;
using System.Collections;
using GridObjectSystem.GadgetSystem;
using System.Collections.Generic;
using System.Linq;
using GridObjectSystem;
using GridObjectSystem.GadgetSystem.YinyangJades;

namespace CardSystem.AllCardHub
{
    public class Gadget_YinyangJadeRound : Card
    {
        [SerializeField] private Gadget jadePrefab;//对应的道具的预制体
        public Gadget GetGadgetPrefab() => jadePrefab;
        [SerializeField] private List<Gadget> gameEntities = new List<Gadget>();//当前道具卡管理的道具实例列表,需要同步操作BattleMessage中的道具列表
        void OnDestroy() //卡牌被清除时一并清除所有阴阳玉
        {
            foreach(Gadget gadget in gameEntities.ToList())
            {
                if(gadget == null) continue;
                BattleMessage.instance?.GetGadgetList()?.Remove(gadget);
                Destroy(gadget.gameObject);
            }
        }

        //用于销毁所有道具
        private IEnumerator DestoryAllGadgetEntities()
        {
            foreach(Gadget gadget in gameEntities.ToList())
            {
                if(gadget == null) continue;
                //等待动画播放完毕
                gadget.GetComponent<AnimTrigger>()?.SetBoolValue("Open", false);
                yield return null;//暂停一帧
                AnimatorStateInfo info = gadget.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
                yield return new WaitForSeconds(info.length / info.speed);//等待动画播放完毕,固定数值大约时0.15
                //将这个道具从BattleMessage中删除
                BattleMessage.instance?.GetGadgetList()?.Remove(gadget);
                //销毁这个道具
                Destroy(gadget.gameObject);
            }
            gameEntities.Clear();//清空道具列表
        }
        /// <summary>
        /// 在将这张卡牌从卡槽中移除时,所有阴阳玉的射击次数-1
        /// </summary>
        /// <returns></returns>
        public override IEnumerator AfterRemoveFromSolt()
        {
            foreach (Gadget gadget in gameEntities)
            {
                if(gadget == null) continue;
                YinyangJade_Round jade = gadget as YinyangJade_Round;
                if(jade == null) continue;
                //yield return jade.OnGadgetEffect();
                jade.SetBulletNumberPreRound(jade.GetBulletNumberPreRound() - 1);
            }
            yield return base.AfterRemoveFromSolt();
        }
        /// <summary>
        /// 在将这张卡牌插入卡槽中时,激活所有已有的阴阳玉,并使其每回合触发的射击次数+1
        /// </summary>
        /// <returns></returns>
        public override IEnumerator AfterInsertToSolt()
        {
            //先激活所有已存在的阴阳玉
            //再设置具体的属性
            Debug.Log("[Gadget_YinyangJadeRound]:"+ name + "have Insert To Solt");
            //yield return base.AfterInsertToSolt();
            foreach (Gadget gadget in gameEntities)
            {
                if(gadget == null) continue;
                YinyangJade_Round jade = gadget as YinyangJade_Round;
                if(jade == null) continue;
                yield return jade.OnGadgetEffect();
                jade.SetBulletNumberPreRound(jade.GetBulletNumberPreRound() + 1);
            }
            yield return base.AfterInsertToSolt();
        }
        /// <summary>
        /// 蓝阴阳玉卡牌在打出后会在角色正前方召唤一个蓝阴阳玉
        /// </summary>
        /// <returns></returns>
        public override IEnumerator AfterPlay()
        {
            Gadget newJade = Instantiate(jadePrefab);
            //设置蓝阴阳玉的基础信息
            //将阴阳玉加入棋盘中
            newJade.transform.SetParent(BattleBoard.instance?.transform);
            //加入控制列表中
            if(!(bool)BattleMessage.instance?.GetGadgetList()?.Contains(newJade)) BattleMessage.instance?.GetGadgetList()?.Add(newJade);
            gameEntities.Add(newJade);
            newJade.GetComponent<AnimTrigger>()?.SetBoolValue("Open", true);//播放打开动画
            //设置阴阳玉的坐标位置
            GridObject player = BattleMessage.instance?.GetControlPlayer();//获取当前控制的玩家实体
            newJade.SetBelongRole(player);//设置道具的归属玩家
            //设置阴阳玉的初始位置
            if(player!=null && newJade!=null) newJade.transform.position = player.transform.position;
            Vector2Int offset = Vector2Int.zero;
            switch (player?.GetDirection())
            {
                case BattleDirection.UP:
                    offset.y = 1;
                    break;
                case BattleDirection.DOWN:
                    offset.y = -1;
                    break;
                case BattleDirection.LEFT:
                    offset.x = -1;
                    break;
                case BattleDirection.RIGHT:
                default:
                    offset.x = 1;
                    break;
            }
            //设置同步位置器的信息
            GadgetPositionToRoleSyncer syncer = newJade.GetComponent<GadgetPositionToRoleSyncer>();
            syncer?.SetGapsToRole(offset);
            syncer?.SetPosSyncOpen(true);//开启位置同步
            syncer?.SetFlySyncOpen(true);//开启飞行状态
            syncer?.SetDirSyncOpen(true);//开启朝向同步
            yield return base.AfterPlay();
        }

        public override IEnumerator AfterTriggerEffective()
        {
            yield return base.AfterTriggerEffective();
        }
        
        public override IEnumerator AfterRoundEnd()
        {
            yield return base.AfterRoundEnd();
        }
        //回合开始时触发
        public override IEnumerator AfterRoundStart()
        {
            yield return base.AfterRoundStart();
        }

        //在你的回合丢弃时触发
        public override IEnumerator AfterDiscard()
        {
            //尝试播放丢弃音效
            //base.AfterDiscard();
            yield return base.AfterDiscard();
        }

        //在抽到卡牌时触发
        public override IEnumerator AfterDraw()
        {
            //尝试播放抽卡音效
            yield return base.AfterDraw();
        }

        //virtual会自动调用父类方法
        // virtual可以用base进行显示调用
        //override不会自动调用父类方法,需要使用base来调用
        /// <summary>
        /// 当道具类的卡牌被消耗时,需要清空其控制的道具列表中的所有道具
        /// </summary>
        /// <returns></returns>
        new public virtual IEnumerator AfterExhaust()
        {
            //优先对道具列表进行处理
            yield return DestoryAllGadgetEntities();
            
        }
    }
}