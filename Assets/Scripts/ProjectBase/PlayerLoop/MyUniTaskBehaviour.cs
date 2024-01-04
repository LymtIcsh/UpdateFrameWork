using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// 自定义的UniTask 生命周期
/// </summary>
public class MyUniTaskBehaviour : MonoBehaviour
{
    /// <summary>
    /// 取消显示Token
    /// </summary>
    protected CancellationTokenSource disableCancecellation = new CancellationTokenSource();
    /// <summary>
    /// 销毁ToKen
    /// </summary>
    protected CancellationTokenSource destroyCancecellation = new CancellationTokenSource();

    protected virtual void OnEnable()
    {
        if (disableCancecellation != null)
            disableCancecellation.Dispose();
        disableCancecellation = new CancellationTokenSource();
    }

    protected virtual void OnDisable() => disableCancecellation.Cancel();

    protected virtual void OnDestroy()
    {
        destroyCancecellation.Cancel();
        destroyCancecellation.Dispose();
    }
}
