using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// 读取 ini 配置文件
/// </summary>
public class MyConfig
{
    private string path;
    public Dictionary<string, string> KeyValues = new Dictionary<string, string>();

    /// <summary>
    /// 读取 ini 文件路径
    /// </summary>
    /// <param name="path">文件路径</param>
    public MyConfig(string path)
    {
        this.path = path;
        StreamReader sr = new StreamReader(path, Encoding.Default);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            if (line.Contains("="))
            {
                string[] kv = line.Split('=');
                string key = kv[0].Trim();
                string v = kv[1].Trim();
                KeyValues.Add(key, v);
            }
        }
    }

    /// <summary>
    /// 写入 ini 文件
    /// </summary>
    /// <param name="section">INI文件中的段落名称</param>
    /// <param name="key">INI文件中的关键字</param>
    /// <param name="value">INI文件中关键字的数值</param>
    /// <param name="path">INI文件的完整路径和名称</param>
    /// <returns></returns>
    [DllImport("kernel32")]
    public static extern long WritePrivateProfileString(string section, string key, string value, string path);
    /// <summary>
    /// 获取 ini 文件
    /// </summary>
    /// <param name="section">INI文件中的段落名称</param>
    /// <param name="key">INI文件中的关键字</param>
    /// <param name="deval">无法读取时候时候的缺省数值</param>
    /// <param name="stringBuilder">用于接收ini文件中键值(数据)的接收缓冲区</param>
    /// <param name="size">pipvalue的缓冲区大小</param>
    /// <param name="path">INI文件的完整路径和名称</param>
    /// <returns></returns>
    [DllImport("kernel32")]
    public static extern int GetPrivateProfileString(string section, string key, string deval, StringBuilder stringBuilder, int size, string path);
    [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr handle, string message, string title, int type);

    /// <summary>
    /// 写入 ini 文件
    /// </summary>
    /// <param name="section">INI文件中的段落名称</param>
    /// <param name="key">INI文件中的关键字</param>
    /// <param name="vaule">INI文件中关键字的数值</param>
    public void WriteIniContent(string section, string key, string vaule) => WritePrivateProfileString(section, key, vaule, path);

    /// <summary>
    /// 读取 ini 文件
    /// </summary>
    /// <param name="section">INI文件中的段落名称</param>
    /// <param name="key">INI文件中的关键字</param>
    /// <returns></returns>
    public string ReadIniContent(string section, string key)
    {
        StringBuilder tmp = new StringBuilder(255);
        int i = GetPrivateProfileString(section, key, "", tmp, 255, path);
        //MessageBox(IntPtr.Zero, path + i + "," + tmp + "," + section + key, "ReadIniContent", 0);
        return tmp.ToString();
    }

    /// <summary>
    /// 判断路径是否正确
    /// </summary>
    /// <returns></returns>
    public bool IsIniPath() => File.Exists(path);
}
