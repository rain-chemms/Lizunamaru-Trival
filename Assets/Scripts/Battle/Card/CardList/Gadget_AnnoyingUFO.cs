using UnityEngine;
using GridObjectSystem.GadgetSystem;
using System.Collections.Generic;
using System.Collections;
using GridObjectSystem.GadgetSystem.Hakerros;
using GridObjectSystem.RoleSystem;
using System.Linq;
using GlobalSystem;
using GridObjectSystem.GadgetSystem.UFOs;
using GridObjectSystem;

namespace CardSystem.AllCardHub
{
    public class Gadget_AnnoyingUFO : Card
    {
        [SerializeField] private AnnoyingUFO ufoPrefab;
        //一张卡牌只能召唤一个UFO
        [SerializeField] private Gadget ufoEntity;
        private void CreateUFO()
        {
            if (ufoEntity != null) return;//已经存在UFO,则返回
            Role role = BattleMessage.instance?.GetControlPlayer();
            if (role == null) return;
            Gadget ufo = Instantiate(ufoPrefab);
            //将UFO加入棋盘
            ufo.transform.SetParent(BattleBoard.instance?.transform);
            //加入控制列表中
            if (!(bool)BattleMessage.instance?.GetGadgetList()?.Contains(ufo)) BattleMessage.instance?.GetGadgetList()?.Add(ufo);
            ufoEntity = ufo;
            //设置UFO初始位置
            ufo.transform.position = (Vector3)role?.transform.position;
            //设置UFO的归属玩家
            ufo.SetBelongRole(role);
            //设置环绕的玩家
            ufo.GetComponent<GridObjectSatellite>()?.SetCenter(role);
            //设置UFO的阵营
            ufo.SetSide((bool)role?.GetSide());
            //尝试播放UFO召唤的音效
            GetComponent<CardVoiceController>()?.PlayCardVoice("Call_UFO");
        }

        private IEnumerator DestroyUFOEntity()
        {
            if (ufoEntity == null) yield break;//不存在八卦炉,则返回
            Gadget gt = ufoEntity;
            ufoEntity = null;
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
            //销毁ufo
            //StartCoroutine(DestoryHakEntity());
            if (ufoEntity != null)
            {
                BattleMessage.instance?.GetGadgetList()?.Remove(ufoEntity);
                Destroy(ufoEntity.gameObject);
                ufoEntity = null;
            }
        }

        public override IEnumerator AfterInsertToSolt()
        {
            CreateUFO();
            return base.AfterInsertToSolt();
        }

        public override IEnumerator AfterRemoveFromSolt()
        {
            yield return DestroyUFOEntity();
            yield return base.AfterRemoveFromSolt();
        }
    }
}