using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源加载模块
/// 1.异步加载
/// 2.委托和 lambda表达式
/// 3.协程
/// 4.泛型
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    //同步加载资源
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        //如果对象是一个GameObject类型的 我把他实例化后 再返回出去 外部 直接使用即可
        if (res is GameObject)
            return GameObject.Instantiate(res);
        else//TextAsset AudioClip
            return res;
    }


    //异步加载资源
    public void LoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        //开启异步加载的协程
        MonoMgr.GetInstance().StartCoroutine(ReallyLoadAsync(name, callback));
    }

    //真正的协同程序函数  用于 开启异步加载对应的资源
    private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;

        if (r.asset is GameObject)
            callback(GameObject.Instantiate(r.asset) as T);
        else
            callback(r.asset as T);
    }

    /// <summary>
    /// UniTask 加载单个资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public async UniTask<Object> UniTaskResLoadAsync<T>(string name) where T: Object
    {
        var Res = await Resources.LoadAsync<T>(name).ToUniTask(Progress.Create<float>
           (
          x => EventCenter.GetInstance().EventTrigger("资源进度条加载", x)
           ));

        return Res;
        
    }

    ///// <summary>
    ///// UniTask 加载路径下所有资源
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="path"></param>
    ///// <param name="callback"></param>
    ///// <returns></returns>
    //public async UniTaskVoid UniTaskResLoadAll<T>(string path, UnityAction<T[]> callback) where T : Object
    //{
    //    await UniTask.Yield();
    //    T[] Res = Resources.LoadAll<T>(path);
    //    callback(Res);
    //}
}
