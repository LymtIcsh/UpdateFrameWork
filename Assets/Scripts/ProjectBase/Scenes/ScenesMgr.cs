using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// 知识点
/// 1.场景异步加载
/// 2.协程
/// 3.委托
/// </summary>
public class ScenesMgr : BaseManager<ScenesMgr>, IProgress<float>
{
    /// <summary>
    /// 切换场景 同步
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name, UnityAction fun)
    {
        //场景同步加载
        SceneManager.LoadScene(name);
        //加载完成过后 才会去执行fun
        fun();
    }

    /// <summary>
    /// 提供给外部的 异步加载的接口方法
    /// </summary>
    /// <param name="name"></param>
    public void LoadSceneAsyn(string name)
    {
        //MonoMgr.GetInstance().StartCoroutine(ReallyLoadSceneAsyn(name, fun));
        ReallyLoadSceneAsync(name).Forget();
    }

    /// <summary>
    /// 为调用者实现 IProgress 接口，因为这样可以没有 lambda 分配。
    /// </summary>
    /// <param name="value"></param>
    public void Report(float value) => EventCenter.GetInstance().EventTrigger(MySceneEnum.Progress, value);


    /// <summary>
    /// 协程异步加载场景
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fun"></param>
    /// <returns></returns>
    private IEnumerator ReallyLoadSceneAsyn(string name, UnityAction fun)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        //可以得到场景加载的一个进度
        while (!ao.isDone)
        {
            //事件中心 向外分发 进度情况  外面想用就用
            EventCenter.GetInstance().EventTrigger(MySceneEnum.Progress, ao.progress);
            //这里面去更新进度条
            yield return ao.progress;
        }
        //加载完成过后 才会去执行fun
        fun();
    }

    /// <summary>
    /// UniTak 异步加载场景
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private async UniTaskVoid ReallyLoadSceneAsync(string name)
    {
        await SceneManager.LoadSceneAsync(name).ToUniTask(progress: this);
    }
}
