using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ResourceManager
{
    /// <summary>
    /// 同步加载资源对象
    /// </summary>
    public static T LoadAssetSync<T>(string path) where T : UnityEngine.Object
    {
        //todo，伪代码 改用自己的资源加载接口！
#if UNITY_EDITOR
            var go = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
#else
            var go = null;
#endif
        return go;
    }

    /// <summary>
    /// 异步加载资源对象
    /// </summary>
    public static void LoadAssetAsync<T>(string path, Action<T> callback) where T : UnityEngine.Object
    {
        //todo，伪代码 改用自己的资源加载接口！
#if UNITY_EDITOR
        var go = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path));
#else
            var go = null;
#endif
        callback?.Invoke(go);
    }

    /// <summary>
    /// 回收资源
    /// </summary>
    public static void Recycle(UnityEngine.GameObject instanceObject, bool forceDestroy = false)
    {
        if (instanceObject == null)
        {
            return;
        }

        //todo，伪代码 改用自己的资源加载接口！
        GameObject.Destroy(instanceObject);
    }
}
