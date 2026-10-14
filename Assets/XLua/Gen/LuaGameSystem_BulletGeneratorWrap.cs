#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class LuaGameSystemBulletGeneratorWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(LuaGameSystem.BulletGenerator);
			Utils.BeginObjectRegister(type, L, translator, 0, 3, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetRole", _m_SetRole);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetBullet", _m_SetBullet);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GenerateBullet", _m_GenerateBullet);
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new LuaGameSystem.BulletGenerator();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to LuaGameSystem.BulletGenerator constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetRole(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                LuaGameSystem.BulletGenerator gen_to_be_invoked = (LuaGameSystem.BulletGenerator)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    GridObjectSystem.RoleSystem.Role _role = (GridObjectSystem.RoleSystem.Role)translator.GetObject(L, 2, typeof(GridObjectSystem.RoleSystem.Role));
                    
                    gen_to_be_invoked.SetRole( _role );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetBullet(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                LuaGameSystem.BulletGenerator gen_to_be_invoked = (LuaGameSystem.BulletGenerator)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<BulletSystem.Bullet>(L, 2)) 
                {
                    BulletSystem.Bullet _bullet = (BulletSystem.Bullet)translator.GetObject(L, 2, typeof(BulletSystem.Bullet));
                    
                    gen_to_be_invoked.SetBullet( _bullet );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)) 
                {
                    string _bulletName = LuaAPI.lua_tostring(L, 2);
                    string _poolName = LuaAPI.lua_tostring(L, 3);
                    
                    gen_to_be_invoked.SetBullet( _bulletName, _poolName );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _bulletName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.SetBullet( _bulletName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaGameSystem.BulletGenerator.SetBullet!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GenerateBullet(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                LuaGameSystem.BulletGenerator gen_to_be_invoked = (LuaGameSystem.BulletGenerator)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 7&& translator.Assignable<UnityEngine.Vector3>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& translator.Assignable<UnityEngine.ForceMode>(L, 4)&& translator.Assignable<System.Action<BulletSystem.Bullet>>(L, 5)&& translator.Assignable<UnityEngine.Vector3>(L, 6)&& translator.Assignable<UnityEngine.Vector3>(L, 7)) 
                {
                    UnityEngine.Vector3 _direction;translator.Get(L, 2, out _direction);
                    float _force = (float)LuaAPI.lua_tonumber(L, 3);
                    UnityEngine.ForceMode _forceMode;translator.Get(L, 4, out _forceMode);
                    System.Action<BulletSystem.Bullet> _selfPostProcess = translator.GetDelegate<System.Action<BulletSystem.Bullet>>(L, 5);
                    UnityEngine.Vector3 _posOffset;translator.Get(L, 6, out _posOffset);
                    UnityEngine.Vector3 _scale;translator.Get(L, 7, out _scale);
                    
                    gen_to_be_invoked.GenerateBullet( _direction, _force, _forceMode, _selfPostProcess, _posOffset, _scale );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 6&& translator.Assignable<UnityEngine.Vector3>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& translator.Assignable<UnityEngine.ForceMode>(L, 4)&& translator.Assignable<System.Action<BulletSystem.Bullet>>(L, 5)&& translator.Assignable<UnityEngine.Vector3>(L, 6)) 
                {
                    UnityEngine.Vector3 _direction;translator.Get(L, 2, out _direction);
                    float _force = (float)LuaAPI.lua_tonumber(L, 3);
                    UnityEngine.ForceMode _forceMode;translator.Get(L, 4, out _forceMode);
                    System.Action<BulletSystem.Bullet> _selfPostProcess = translator.GetDelegate<System.Action<BulletSystem.Bullet>>(L, 5);
                    UnityEngine.Vector3 _posOffset;translator.Get(L, 6, out _posOffset);
                    
                    gen_to_be_invoked.GenerateBullet( _direction, _force, _forceMode, _selfPostProcess, _posOffset );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 5&& translator.Assignable<UnityEngine.Vector3>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& translator.Assignable<UnityEngine.ForceMode>(L, 4)&& translator.Assignable<System.Action<BulletSystem.Bullet>>(L, 5)) 
                {
                    UnityEngine.Vector3 _direction;translator.Get(L, 2, out _direction);
                    float _force = (float)LuaAPI.lua_tonumber(L, 3);
                    UnityEngine.ForceMode _forceMode;translator.Get(L, 4, out _forceMode);
                    System.Action<BulletSystem.Bullet> _selfPostProcess = translator.GetDelegate<System.Action<BulletSystem.Bullet>>(L, 5);
                    
                    gen_to_be_invoked.GenerateBullet( _direction, _force, _forceMode, _selfPostProcess );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& translator.Assignable<UnityEngine.Vector3>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& translator.Assignable<UnityEngine.ForceMode>(L, 4)) 
                {
                    UnityEngine.Vector3 _direction;translator.Get(L, 2, out _direction);
                    float _force = (float)LuaAPI.lua_tonumber(L, 3);
                    UnityEngine.ForceMode _forceMode;translator.Get(L, 4, out _forceMode);
                    
                    gen_to_be_invoked.GenerateBullet( _direction, _force, _forceMode );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& translator.Assignable<UnityEngine.Vector3>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    UnityEngine.Vector3 _direction;translator.Get(L, 2, out _direction);
                    float _force = (float)LuaAPI.lua_tonumber(L, 3);
                    
                    gen_to_be_invoked.GenerateBullet( _direction, _force );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& translator.Assignable<UnityEngine.Vector3>(L, 2)) 
                {
                    UnityEngine.Vector3 _direction;translator.Get(L, 2, out _direction);
                    
                    gen_to_be_invoked.GenerateBullet( _direction );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaGameSystem.BulletGenerator.GenerateBullet!");
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
