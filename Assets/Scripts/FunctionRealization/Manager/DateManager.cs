using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateManager : SingletonAutoMono<DateManager>
{
    public float MoveSpeed;

    /// <summary>
    /// 按钮响应队列出队间隔
    /// </summary>
    public int ButtonResponseInterval;

    /// <summary>
    /// Config配置文件路径
    /// </summary>
    string Config_File = Application.streamingAssetsPath + "/Config/" + "SetingConfig.ini";

    private void Awake()
    {
        MyConfig myConfig = new MyConfig(Config_File);

        MoveSpeed = float.Parse(myConfig.KeyValues["MoveSpeed"]);
        ButtonResponseInterval = Convert.ToInt32(myConfig.KeyValues["ButtonResponseInterval"]);
    }
}
