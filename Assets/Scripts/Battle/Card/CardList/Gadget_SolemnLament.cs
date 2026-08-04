using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GridObjectSystem;
using GridObjectSystem.GadgetSystem;
using GridObjectSystem.RoleSystem;

namespace CardSystem.AllCardHub
{
    public class Gadget_SolemnLament : Card
    {
        [SerializeField] private Gadget whiteGun;//白枪
        public Gadget GetWhiteGunPrefab() => whiteGun;
        [SerializeField] private Vector2Int whiteGunOffset;//白枪偏移
        public Vector2Int GetWhiteGunOffset() => whiteGunOffset;
        [SerializeField] private Gadget blackGun;//黑枪
        public Gadget GetBlackGunPrefab() => blackGun;
        [SerializeField] private Vector2Int blackGunOffset;//黑枪偏移 
        public Vector2Int GetBlackGunOffset() => blackGunOffset;
        [SerializeField] private List<Gadget> gunEntities = new List<Gadget>();//管理的枪械实体
        void OnDestroy() //卡牌被清除时一并清除所有枪械
        {
            foreach(Gadget gadget in gunEntities.ToList())
            {
                if(gadget == null) continue;
                BattleMessage.instance?.GetGadgetList()?.Remove(gadget);
                Destroy(gadget.gameObject);
            }
        }

        public override IEnumerator AfterDiscard()
        {
            //被丢弃时触发一次双枪射击
            foreach(Gadget gadget in gunEntities)
            {
                if(gadget == null) continue;
                yield return gadget.OnGadgetEffect();
            }
            yield return base.AfterDiscard();
        }

        public override IEnumerator AfterExhaust()
        {
            yield return base.AfterRemoveFromSolt();
            yield return DestroyAllEntities();
        }

        public override IEnumerator AfterRemoveFromSolt()
        {
            yield return base.AfterRemoveFromSolt();    
            yield return DestroyAllEntities();
        }

        public override IEnumerator AfterInsertToSolt()
        {
            yield return base.AfterInsertToSolt();
            CreateTwoGunToPlayer();
        }

        private IEnumerator DestroyAllEntities()
        {
             foreach(Gadget gadget in gunEntities.ToList())
            {
                if(gadget == null) continue;
                //等待动画播放完毕
                gadget.GetComponent<AnimTrigger>()?.SetBoolValue("Open", false);
                yield return null;//暂停一帧
                AnimatorStateInfo info = gadget.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
                yield return new WaitForSeconds(0.1f);//等待动画播放完毕
                //将这个道具从BattleMessage中删除
                BattleMessage.instance?.GetGadgetList()?.Remove(gadget);
                //销毁这个道具
                Destroy(gadget.gameObject);
            }
            gunEntities.Clear();//清空道具列表
        }
        //创建双枪
        private void CreateTwoGunToPlayer()
        {
            if(whiteGun == null || blackGun == null || BattleMessage.instance == null) return;
            //获取卡牌正在控制的玩家
            Role role = BattleMessage.instance?.GetControlPlayer();
            if(role == null) return ;
            Gadget wGun = Instantiate(whiteGun);
            Gadget bGun = Instantiate(blackGun);
            //将黑白双枪加入棋盘
            wGun.transform.SetParent(BattleBoard.instance?.transform);
            bGun.transform.SetParent(BattleBoard.instance?.transform);
            //加入控制列表中
            if(!(bool)BattleMessage.instance?.GetGadgetList()?.Contains(wGun))BattleMessage.instance?.GetGadgetList()?.Add(wGun);
            if(!(bool)BattleMessage.instance?.GetGadgetList()?.Contains(bGun))BattleMessage.instance?.GetGadgetList()?.Add(bGun);
            gunEntities.Add(wGun);
            gunEntities.Add(bGun);
            //设置初始位置
            wGun.transform.position = role.transform.position;
            bGun.transform.position = role.transform.position;
            //设置wGun和bGun的归属玩家
            bGun.SetBelongRole(role);
            wGun.SetBelongRole(role);
            //设置双枪的玩家位置同步器
            GadgetPositionToRoleSyncer bSyncer = bGun.GetComponent<GadgetPositionToRoleSyncer>();
            bSyncer?.SetPosSyncOpen(true);
            bSyncer?.SetDirSyncOpen(true);
            bSyncer?.SetFlySyncOpen(true);
            bSyncer?.SetGapsToRole(blackGunOffset);
            GadgetPositionToRoleSyncer wSyncer = wGun.GetComponent<GadgetPositionToRoleSyncer>();
            wSyncer?.SetPosSyncOpen(true);
            wSyncer?.SetDirSyncOpen(true);
            wSyncer?.SetFlySyncOpen(true);
            wSyncer?.SetGapsToRole(whiteGunOffset);
        }
    }

}