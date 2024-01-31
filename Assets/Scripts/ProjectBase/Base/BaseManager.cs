using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

//1.C#中 泛型的知识
//2.设计模式中 单例模式的知识
public class BaseManager<T> where T:new()
{
    private static T instance;

    public static T GetInstance()
    {
        if (instance == null)
            instance = new T();
        return instance;
    }


}

#region 线程加锁，防止外面new 但有性能消耗，多人开发使用
//
// public abstract class BaseManager<T> where T : class
// {
//     private static T instance = null;
//
//     /// <summary>
//     /// 多线程安全机制 加锁
//     /// </summary>
//     protected static readonly object locker = new object();
//
//     public static T GetInstance
//     {
//         get
//         {
//             if (instance == null)
//             {
//                 lock (locker)
//                 {
//                     if (instance == null)
//                     {
//                         //利用反射得到无参私有的构造函数来用于对象的实例化
//                        // instance = Activator.CreateInstance(typeof(T),true) as T;
//                         ConstructorInfo info = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
//                             null, Type.EmptyTypes, null);
//
//                         if (info != null)
//                             instance = info.Invoke(null) as T;
//                         else
//                             Debug.LogError("没有得到对应的无参构造函数");
//                     }
//                 }
//             }
//
//             return instance;
//         }
//     }
//
//     /// <summary>
//     /// 构造函数
//     /// 避免外界new
//     /// </summary>
//     protected BaseManager()
//     {
//     }
// }

#endregion