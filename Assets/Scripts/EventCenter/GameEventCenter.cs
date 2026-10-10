using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace GameEventSystem
{
    //事件中心
    public class GameEventCenter
    {
        private static GameEventCenter instance;

        public static GameEventCenter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameEventCenter();
                }
                return instance;
            }
        }

        //事件列表字典
        private Dictionary<string, IGameEvenetInfo> eventDict = new Dictionary<string, IGameEvenetInfo>();

        //添加订阅事件
        public void AddEventListener(string eventName, Action eventAct)
        {
            if (eventDict.ContainsKey(eventName))
            {
                //添加额外的订阅事件
                (eventDict[eventName] as GameEvnetInfo).actions += eventAct;
            }
            else
            {
                //创建订阅事件并添加到列表中
                eventDict.Add(eventName,new  GameEvnetInfo(eventAct));
            }
        }

        //激活事件
        public void EventTrigger(string eventName)
        {
            if (eventDict.ContainsKey(eventName))
            {
                //触发所有订阅当前事件的物体
                if((eventDict[eventName] as GameEvnetInfo).actions != null)
                {
                    (eventDict[eventName] as GameEvnetInfo).actions.Invoke();
                }
                
            }
        }

        //移除某一个事件的激活功能
        public void RemoveEventListener(string eventName, Action eventAct)
        {
            if(eventDict.ContainsKey(eventName))
            {
                (eventDict[eventName] as GameEvnetInfo).actions -= eventAct;
            }
        }

        //清除某一个事件映射
        public void RemoveEvent(string eventName)
        {
            if(eventDict.ContainsKey(eventName))
            {
                eventDict[eventName] = null;
                eventDict.Remove(eventName);
            }
        }

        public void Clear()
        {
            eventDict.Clear();
        }
    }

    //订阅者接口
    public interface IGameEvenetInfo{}

    //订阅响应体:无参数相应体
    public class GameEvnetInfo : IGameEvenetInfo
    {
        public Action actions;

        public GameEvnetInfo(Action act)
        {
            actions += act;
        }
    }

}