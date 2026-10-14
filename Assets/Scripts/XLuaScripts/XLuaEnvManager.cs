using UnityEngine;
using XLua;

namespace LuaGameSystem
{
    //XLua环境管理器
    //是单例物体
    public class XLuaEnvManager : MonoBehaviour
    {
        public static XLuaEnvManager instance;
        private LuaEnv env;
        public LuaEnv Env { get => env; set => env = value; }
        void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            env = new LuaEnv();//初始化全局XLua环境
            env?.DoString("print('[XLuaEnvManager]: Global XLuaEnv Created!')");
        }

        //释放全局Lua环境
        void OnDestroy()
        {
            env?.Dispose();
        }
    }
}
