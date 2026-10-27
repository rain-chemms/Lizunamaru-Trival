using BulletSystem;
using GridObjectSystem.RoleSystem;
using UnityEngine.AddressableAssets;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XLua;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using UnityEditor.SceneManagement;
using GridObjectSystem;

namespace LuaGameSystem
{
    //用于动态Lua代码的子弹生成器
    [LuaCallCSharp]
    public class BulletGenerator
    {
        public BulletGenerator() { }
        private GridObject generateObject = null;
        private Bullet bulletPrefab = null;
        public void SetGenerateObject(GridObject role) => generateObject = role;
        public void SetBullet(Bullet bullet) => bulletPrefab = bullet;

        //通过资源系统的子弹名获取子弹
        public IEnumerator SetBullet(string bulletName, string poolName = null)
        {
            var keys = new List<string> { "Bullet" };
            if (!string.IsNullOrEmpty(poolName)) keys.Add("Bullet.Pool." + poolName);
            // 启动异步加载，并在加载完成后执行回调
            var handle = Addressables.LoadAssetsAsync<GameObject>(keys, null, Addressables.MergeMode.Intersection);

            handle.Completed += (asyncOp) =>
            {
                if (asyncOp.Status == AsyncOperationStatus.Succeeded)
                {
                    List<GameObject> pfbs = asyncOp.Result.ToList();
                    foreach (GameObject p in pfbs)
                    {
                        if (p == null) continue;

                        Bullet bullet = p.GetComponent<Bullet>();
                        // 注意：这里比较的是 GameObject 的 name 属性
                        if (bullet != null && p.name.Equals(bulletName))
                        {
                            bulletPrefab = bullet;
                            break;
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"[XLuaBulletGenerator]: Card Prefab Load Failed: {asyncOp.OperationException}");
                }
                handle.Release();
            };
            yield return handle;
        }

        public void SetBulletSync(string bulletName, string poolName = null)
        {
            var keys = new List<string> { "Bullet" };
            if (!string.IsNullOrEmpty(poolName)) keys.Add("Bullet.Pool." + poolName);

            var handle = Addressables.LoadAssetsAsync<GameObject>(
                keys, null, Addressables.MergeMode.Intersection);

            handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                List<GameObject> pfbs = handle.Result.ToList();
                foreach (GameObject p in pfbs)
                {
                    if (p == null) continue;

                    Bullet bullet = p.GetComponent<Bullet>();
                    if (bullet != null && p.name.Equals(bulletName))
                    {
                        bulletPrefab = bullet;
                        break;
                    }
                }
            }

            handle.Release();
        }
        
        public void GenerateBullet(Vector3 direction, float force = default, ForceMode forceMode = default, Action<Bullet> selfPostProcess = null, Vector3 posOffset = default, Vector3 scale = default)
        {
            if (bulletPrefab == null || generateObject == null) return;
            Bullet bt = UnityEngine.Object.Instantiate(bulletPrefab);
            if (scale != default) bt.SetScale(scale);//非默认情况设置大小
            bt.SetSide(generateObject.GetSide());
            Rigidbody rRb = generateObject.GetRigidBody();
            Rigidbody bRb = bt?.GetRigidBody();
            if (rRb != null && bRb != null)
            {
                bRb.transform.position = rRb.worldCenterOfMass + posOffset;
            }
            if (force != default) bt?.SetForce(force);
            if (forceMode != default) bt?.SetForceMode(forceMode);//默认时不改变子弹的forceMode
            bt?.SetDirection(direction);

            selfPostProcess?.Invoke(bt);//尝试触发后处理
            BattleMessage.instance?.BulletPostProcess?.Invoke(bt);//尝试触发后处理
        }
    }
}