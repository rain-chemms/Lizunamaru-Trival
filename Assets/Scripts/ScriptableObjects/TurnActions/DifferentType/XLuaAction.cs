using System;
using System.Collections;
using LuaGameSystem;
using UnityEngine;
using XLua;

namespace GridObjectSystem.RoleSystem.AutoSystem
{
    [CreateAssetMenu(fileName = "InTurnAutoAction", menuName = "InTurnAutoAction/Actions/XLuaAction")]
    public class XLuaAction : TurnAction
    {
        [SerializeField] private bool readFromFile = false;//是否加载lua文件
        [ShowIfBoolAttribute("readFromFile",true)] [TextArea(3, 10)] [SerializeField] private string defaultLuaCode;//存储对应的LuaCode去执行
        [ShowIfBoolAttribute("readFromFile")] [SerializeField] private TextAsset luaFile = null;
        //制作一种可以手动编程和调用的脚本
        public override IEnumerator Excute(Role role)
        {
            
            string luaCode = GetLuaCode();

            if (!string.IsNullOrEmpty(luaCode))
            {
                XLuaEnvManager.instance?.Env.Global.Set("btGrRole",role);
                try{
                    XLuaEnvManager.instance?.Env.DoString(luaCode);
                }
                catch(Exception e)
                {
                    Debug.LogError($"[XLuaAction]: Lua Code Excute Bugs!\nError Message: { e.Message }");
                }
            }
            
            yield return base.Excute(role);
        }

        private string GetLuaCode()
        {
            if (readFromFile && luaFile != null)
            {
                return luaFile.text;
            }

            return defaultLuaCode;
        }
    }
}