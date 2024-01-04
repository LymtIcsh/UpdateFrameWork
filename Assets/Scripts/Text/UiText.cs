using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiText : BasePanel
{
    public int m_BtnClickInterval;

    public string mess;

    protected override void Awake()
    {
        BtnClickInterval = m_BtnClickInterval;
        base.Awake();
    }

    protected override void OnClick(string btnName)
    {
        Debug.Log(mess);
    }
}
