using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using CardSystem;
using System.Linq;

namespace GlobalSystem
{
    //手牌打出器,用于关联输入系统进行键盘操作
    public class HandCardPlayer : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputAsset;//输入系统资源 
        private InputActionMap cardPlayMap;
        private InputAction card1;
        private InputAction card2;
        private InputAction card3;
        private InputAction card4;
        private InputAction card5;
        private InputAction card6;
        private InputAction card7;
        private InputAction card8;
        private InputAction card9;
        private InputAction card10;
        private InputAction playCard;

        void OnEnable()
        {
            InitInput();
            LinkFunc();   
        }

        void OnDisable()
        {
            DisLinkFunc();
            ReleaseInput();
        }
        
        private void InitInput()
        {
            cardPlayMap = inputAsset?.FindActionMap("HandCardPlayer");
            card1 = cardPlayMap?.FindAction("Card1");
            card2 = cardPlayMap?.FindAction("Card2");
            card3 = cardPlayMap?.FindAction("Card3");
            card4 = cardPlayMap?.FindAction("Card4");
            card5 = cardPlayMap?.FindAction("Card5");
            card6 = cardPlayMap?.FindAction("Card6");
            card7 = cardPlayMap?.FindAction("Card7");
            card8 = cardPlayMap?.FindAction("Card8");
            card9 = cardPlayMap?.FindAction("Card9");
            card10 = cardPlayMap?.FindAction("Card10");
            playCard = cardPlayMap?.FindAction("PlayCard");
            cardPlayMap?.Enable();//启用输入系统
        }

        private void ReleaseInput()
        {
            cardPlayMap?.Disable();//禁用输入系统
            cardPlayMap = null;
            card1 = null;
            card2 = null;
            card3 = null;
            card4 = null;
            card5 = null;
            card6 = null;
            card7 = null;
            card8 = null;
            card9 = null;
            card10 = null;
            playCard = null;
        }
        
        private void LinkFunc()
        {
            if(card1 != null)
            {               
                card1.started += OnCard1Start;
                card1.canceled += OnCard1Cancel;
            }   
            if(card2 != null)
            {
                card2.started += OnCard2Start;
                card2.canceled += OnCard2Cancel;
            }      
            if(card3 != null)
            {
                card3.started += OnCard3Start;
                card3.canceled += OnCard3Cancel;
            }
            if(card4 != null)
            {
                card4.started += OnCard4Start;
                card4.canceled += OnCard4Cancel;
            }
            if(card5 != null)
            {
                card5.started += OnCard5Start;
                card5.canceled += OnCard5Cancel;
            }
            if(card6 != null)
            {
                card6.started += OnCard6Start;
                card6.canceled += OnCard6Cancel;
            }
            if(card7 != null)
            {
                card7.started += OnCard7Start;
                card7.canceled += OnCard7Cancel;
            }
            if(card8 != null)
            {
                card8.started += OnCard8Start;
                card8.canceled += OnCard8Cancel;
            }
            if(card9 != null)
            {
                card9.started += OnCard9Start;
                card9.canceled += OnCard9Cancel;
            }
            if(card10 != null)
            {
                card10.started += OnCard10Start;
                card10.canceled += OnCard10Cancel;
            }
            if(playCard != null)
            {
                playCard.performed += OnPlayCard;
            }
        }

        private void DisLinkFunc()
        {
            if(card1 != null)
            {               
                card1.started -= OnCard1Start;
                card1.canceled -= OnCard1Cancel;
            }
            if(card2 != null)
            {
                card2.started -= OnCard2Start;
                card2.canceled -= OnCard2Cancel;
            }
            if(card3 != null)
            {
                card3.started -= OnCard3Start;
                card3.canceled -= OnCard3Cancel;
            }
            if(card4 != null)
            {
                card4.started -= OnCard4Start;
                card4.canceled -= OnCard4Cancel;
            }
            if(card5 != null)
            {
                card5.started -= OnCard5Start;
                card5.canceled -= OnCard5Cancel;
            }
            if(card6 != null)
            {
                card6.started -= OnCard6Start;
                card6.canceled -= OnCard6Cancel;
            }
            if(card7 != null)
            {
                card7.started -= OnCard7Start;
                card7.canceled -= OnCard7Cancel;
            }
            if(card8 != null)
            {
                card8.started -= OnCard8Start;
                card8.canceled -= OnCard8Cancel;
            }
            if(card9 != null)
            {
                card9.started -= OnCard9Start;
                card9.canceled -= OnCard9Cancel;
            }
            if(card10 != null)
            {
                card10.started -= OnCard10Start;
                card10.canceled -= OnCard10Cancel;
            }
            if(playCard != null)
            {
                playCard.performed -= OnPlayCard;
            }
        }

        [SerializeField] private int cardIndex = -1;//手牌索引初始为-1代表没有选择
        public int GetCardIndex() => cardIndex;
        [SerializeField] private bool isSelected = false;
        public bool IsSelected() => isSelected;
        public void SetSelected(bool selected) => isSelected = selected;
        
        //按下的时候设置卡牌的选择索引,并将相应卡牌的CardHoverChecker的isHovering设置为true
        //抬起时,将其的isHovering设置为false,并设置CardHandler的isDraging设置为true
        private void SetHandCardIsHovering(int index,bool isHovering)
        {
            List<Card> handCards = BattleMessage.instance?.GetHandCardList()?.ToList();
            int count = handCards.Count;
            if(index < 0 || index >= count) return;//索引越界不执行
            Card targetCard = handCards[index];//获取索引对应的卡牌
            targetCard.GetComponent<CardHoverChecker>()?.SetIsHovering(isHovering);
        }

        //被拖拽的卡只能有一张,但是isHovering可以设置多张
        private void SetHandCardIsDraging(int index,bool isDragging)
        {
            List<Card> handCards = BattleMessage.instance?.GetHandCardList()?.ToList();
            int count = handCards.Count;
            for(int i = 0 ; i < count; i++)
            {
                if(i == index) continue;
                CardHandler handler = handCards[i].GetComponent<CardHandler>();
                handler?.SetIsDragging(false);//抬起时将所有卡牌isDraging设置为false
            }
            if(index < 0 || index >= count) return;//索引越界不执行
            Card targetCard = handCards[index];//获取索引对应的卡牌
            CardHandler targetHandler = targetCard?.GetComponent<CardHandler>();
            targetHandler?.SetIsDragging(isDragging);
            targetHandler?.SetDragOffset(Vector2.zero);
            targetHandler?.SetGrabOffset(Vector2.zero);
            targetHandler?.SetDragTarget(Vector2.zero);
        }

        //方法函数
        private void CheckAndSetSelected(int index)
        {
            //检测上一次的索引,以便确认是否处于卡牌选中状态
            if(cardIndex == index)//上一次选择了同一张卡牌
            {
                isSelected = !isSelected;
            }
            else //上上一次选择了其他卡牌
            {
                isSelected = true;
            }
            cardIndex = index;//抬起来的时候设置对应的索引
            SetHandCardIsHovering(index,false);
            SetHandCardIsDraging(cardIndex,isSelected);
            
        }

        private void OnCard1Start(InputAction.CallbackContext context)
        {
            SetHandCardIsHovering(0,true);
        }

        private void OnCard1Cancel(InputAction.CallbackContext context)
        {
            CheckAndSetSelected(0);
        }

        private void OnCard2Start(InputAction.CallbackContext context )
        {
            SetHandCardIsHovering(1,true);
        }

        private void OnCard2Cancel(InputAction.CallbackContext context)
        {
            CheckAndSetSelected(1);
        }

        private void OnCard3Start(InputAction.CallbackContext context)
        {
            SetHandCardIsHovering(2,true);
        }

        private void OnCard3Cancel(InputAction.CallbackContext context)
        {
            CheckAndSetSelected(2);
        }

        private void OnCard4Start(InputAction.CallbackContext context)
        {
            SetHandCardIsHovering(3,true);
        }

        private void OnCard4Cancel(InputAction.CallbackContext context)
        {
            CheckAndSetSelected(3);
        }

        private void OnCard5Start(InputAction.CallbackContext context)
        {
            SetHandCardIsHovering(4,true);
        }

        private void OnCard5Cancel(InputAction.CallbackContext context)
        {
            CheckAndSetSelected(4);
        }

        private void OnCard6Start(InputAction.CallbackContext context)
        {
            SetHandCardIsHovering(5,true);
        }

        private void OnCard6Cancel(InputAction.CallbackContext context)
        {
            CheckAndSetSelected(5);
        }

        private void OnCard7Start(InputAction.CallbackContext context)
        {
            SetHandCardIsHovering(6,true);
        }

        private void OnCard7Cancel(InputAction.CallbackContext context)
        {
            CheckAndSetSelected(6);
        }

        private void OnCard8Start(InputAction.CallbackContext context)
        {
            SetHandCardIsHovering(7,true);
        }

        private void OnCard8Cancel(InputAction.CallbackContext context)
        {
            CheckAndSetSelected(7);
        }   

        private void OnCard9Start(InputAction.CallbackContext context)
        {
            SetHandCardIsHovering(8,true);
        }

        private void OnCard9Cancel(InputAction.CallbackContext context)
        {
            CheckAndSetSelected(8);
        }

        private void OnCard10Start(InputAction.CallbackContext context)
        {
            SetHandCardIsHovering(9,true);
        }   

        private void OnCard10Cancel(InputAction.CallbackContext context)
        {
            CheckAndSetSelected(9);
        }
        
        private void OnPlayCard(InputAction.CallbackContext context)
        {
            if (!isSelected)
            {
                Debug.LogWarning("[HandCardPlayer]: Not Selected Card!");
                return;//未选择卡牌时不执行
            }
            //只有当前卡牌执行完毕之后才能加入新的卡牌
            CardPlayArea area = BattleMessageDisplayer.instance?.GetCardPlayArea();
            if((bool)area?.IsExecuting())
            {
                Debug.LogWarning("[HandCardPlayer]: Last Card Play not End!");
                return;
            }

            //尝试获取对应的卡牌
            List<Card> handCards = BattleMessage.instance?.GetHandCardList()?.ToList();
            if(cardIndex < 0 || cardIndex >= handCards?.Count)
            {
                Debug.LogWarning("[HandCardPlayer]: Index Out Of Range!");
                return;//索引越界不执行    
            }
            //将目标卡牌的IsDragging设置为false
            SetHandCardIsDraging(cardIndex,false);
            isSelected = false;
            Card targetCard = handCards[cardIndex];
            cardIndex = -1;//重置索引
            if((bool)BattleMessage.instance?.GetHandCardList()?.Contains(targetCard)) BattleMessage.instance?.GetHandCardList()?.Remove(targetCard);
            //将卡牌加入卡牌打出区域
            area?.AddCard(targetCard);    
        }   
    }

}
