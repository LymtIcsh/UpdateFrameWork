using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 面板基类 
/// 帮助我门通过代码快速的找到所有的子控件
/// 方便我们在子类中处理逻辑 
/// 节约找控件的工作量
/// </summary>
public class BasePanel : MyUniTaskBehaviour
{
    //通过里式转换原则 来存储所有的控件
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

    /// <summary>
    /// 按钮点击间隔
    /// </summary>
    protected  int BtnClickInterval = 0;



    // Use this for initialization
    protected virtual void Awake()
    {
        FindChildrenControl<Button>(destroyCancecellation.Token, BtnClickInterval).Forget();
        FindChildrenControl<Image>(destroyCancecellation.Token, BtnClickInterval).Forget();
        FindChildrenControl<RawImage>(destroyCancecellation.Token, BtnClickInterval).Forget();
        FindChildrenControl<Text>(destroyCancecellation.Token, BtnClickInterval).Forget();
        FindChildrenControl<TextMeshProUGUI>(destroyCancecellation.Token, BtnClickInterval).Forget();
        FindChildrenControl<Toggle>(destroyCancecellation.Token, BtnClickInterval).Forget();
        FindChildrenControl<Slider>(destroyCancecellation.Token, BtnClickInterval).Forget();
        FindChildrenControl<ScrollRect>(destroyCancecellation.Token, BtnClickInterval).Forget();
        FindChildrenControl<InputField>(destroyCancecellation.Token, BtnClickInterval).Forget();
    }

    /// <summary>
    /// 显示自己
    /// </summary>
    public virtual void ShowMe()
    {

    }

    /// <summary>
    /// 隐藏自己
    /// </summary>
    public virtual void HideMe()
    {

    }

    protected virtual void OnClick(string btnName)
    {

    }

    protected virtual void OnValueChanged(string toggleName, bool value)
    {

    }

    /// <summary>
    /// 得到对应名字的对应控件脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    protected T GetControl<T>(string controlName) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(controlName))
        {
            for (int i = 0; i < controlDic[controlName].Count; ++i)
            {
                if (controlDic[controlName][i] is T)
                    return controlDic[controlName][i] as T;
            }
        }

        return null;
    }

    /// <summary>
    /// 找到子对象的对应控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="token"></param>
    /// <param name="btnClickInterval"> 按钮点击间隔</param>
    /// <returns></returns>
    private async UniTaskVoid FindChildrenControl<T>(CancellationToken token, int btnClickInterval = 0) where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        for (int i = 0; i < controls.Length; ++i)
        {
            string objName = controls[i].gameObject.name;
            if (controlDic.ContainsKey(objName))
                controlDic[objName].Add(controls[i]);
            else
                controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
            await UniTask.Yield();
            //如果是按钮控件
            if (controls[i] is Button)
            {
                (controls[i] as Button).OnClickAsAsyncEnumerable().ForEachAwaitAsync(async (_) =>
                {
                    OnClick(objName);
                    //按钮点击间隔
                    if (btnClickInterval > 0)
                        await UniTask.Delay(TimeSpan.FromSeconds(btnClickInterval), cancellationToken: token);
                }, token);

                //(controls[i] as Button).onClick.AddListener(()=>
                //{
                //    OnClick(objName);
                //});
            }
            //如果是单选框或者多选框
            else if (controls[i] is Toggle)
            {
                (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                {
                    OnValueChanged(objName, value);
                });
            }
        }
    }
}
